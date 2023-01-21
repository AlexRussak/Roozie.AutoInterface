using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roozie.AutoInterface;

internal static class InterfaceExtractor
{
    private static readonly ImmutableArray<string> ObjectMethods = new[]
    {
        nameof(ToString),
        nameof(GetHashCode),
        nameof(Equals),
    }.ToImmutableArray();

    public static InterfaceToGenerate? ProcessClass(AttributeData attributeData,
        ClassDeclarationSyntax? classDeclarationSyntax, SemanticModel semanticModel,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (classDeclarationSyntax == null)
        {
            return null;
        }

        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax, ct) is not { } classSymbol)
        {
            return null;
        }

        var settings = GetSettings(attributeData);
        var methods = new List<MethodToGenerate>();
        var properties = new List<PropertyToGenerate>();
        foreach (var member in classSymbol.GetMembers())
        {
            if (member.DeclaredAccessibility != Accessibility.Public ||
                member.IsStatic ||
                member.IsImplicitlyDeclared)
            {
                continue;
            }

            var memberContainsAttribute = false;
            var memberAttrs = member.GetAttributes();
            if (memberAttrs.Any(a =>
                    string.Equals(a.AttributeClass?.Name, nameof(AddToInterfaceAttribute),
                        StringComparison.Ordinal)))
            {
                memberContainsAttribute = true;
            }

            if (member is IMethodSymbol methodSymbol &&
                (memberContainsAttribute || settings.IncludeMethods))
            {
                if (ObjectMethods.Contains(methodSymbol.Name, StringComparer.Ordinal) ||
                    methodSymbol.MethodKind != MethodKind.Ordinary)
                {
                    continue;
                }

                methods.Add(ConvertMethod(methodSymbol, ct));
            }
            else if (member is IPropertySymbol propertySymbol &&
                     (memberContainsAttribute || settings.IncludeProperties))
            {
                properties.Add(ConvertProperty(propertySymbol, ct));
            }
        }

        var root = (CompilationUnitSyntax)classDeclarationSyntax.SyntaxTree.GetRoot(ct);
        var usings = root.Usings
            .Select(u => u.Name.ToString())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .OrderBy(u => u, StringComparer.Ordinal)
            .ToImmutableArray();

        var classDoc = classSymbol.GetDocumentationCommentXml(cancellationToken: ct);

        var implementPartial = classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                               settings.ImplementOnPartial;
        return new(
            classSymbol.Name,
            settings.InterfaceName ?? "I" + classSymbol.Name,
            classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToString(),
            usings,
            methods.ToArray(),
            properties.ToArray(),
            classDoc,
            implementPartial,
            classDeclarationSyntax.GetLocation(),
            classSymbol.IsStatic ? ErrorType.StaticClass : null
        );
    }

    private static GeneratorSettings GetSettings(AttributeData attributeData)
    {
        string? interfaceName = null;
        bool? includeMethods = null;
        bool? includeProperties = null;
        bool? implementOnPartial = null;

        foreach (var kvp in attributeData.NamedArguments)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            if (string.Equals(key, nameof(AutoInterfaceAttribute.Name), StringComparison.Ordinal)
                && value.Value?.ToString() is { } s
                && !string.IsNullOrWhiteSpace(s))
            {
                interfaceName = s;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.IncludeMethods), StringComparison.Ordinal)
                && value.Value is bool methodsFlag)
            {
                includeMethods = methodsFlag;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.IncludeProperties), StringComparison.Ordinal)
                && value.Value is bool propertiesFlag)
            {
                includeProperties = propertiesFlag;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.ImplementOnPartial), StringComparison.Ordinal)
                && value.Value is bool partialFlag)
            {
                implementOnPartial = partialFlag;
            }
        }

        return new(interfaceName, includeMethods ?? true, includeProperties ?? true, implementOnPartial ?? true);
    }

    private static MethodToGenerate ConvertMethod(IMethodSymbol method, CancellationToken ct)
    {
        var parameters = method.Parameters.Select(ConvertParameter).ToArray();

        return new(method.Name, method.ReturnType.ToDisplayString(),
            parameters,
            method.GetDocumentationCommentXml(cancellationToken: ct));
    }

    private static PropertyToGenerate ConvertProperty(IPropertySymbol property, CancellationToken ct)
    {
        var hasGetter = property.GetMethod is { DeclaredAccessibility: Accessibility.Public };

        SetPropertyType? setPropertyType = null;
        if (property.SetMethod is { DeclaredAccessibility: Accessibility.Public })
        {
            setPropertyType = property.SetMethod.IsInitOnly ? SetPropertyType.Init : SetPropertyType.Set;
        }

        var name = property.Name;
        if (property.IsIndexer)
        {
            name = name.TrimEnd('[', ']');
        }

        return new(name, property.Type.ToDisplayString(),
            property.Parameters.Select(ConvertParameter).ToArray(),
            hasGetter, setPropertyType,
            property.GetDocumentationCommentXml(cancellationToken: ct));
    }

    private static ParameterToGenerate ConvertParameter(IParameterSymbol parameter) =>
        new(parameter.Name, parameter.Type.ToDisplayString());
}
