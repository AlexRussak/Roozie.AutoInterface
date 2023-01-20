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
            if (member.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            var memberContainsAttribute = false;
            if (settings is { IncludeMethods: true, IncludeProperties: true })
            {
                memberContainsAttribute = true;
            }
            else
            {
                var memberAttrs = member.GetAttributes();
                if (memberAttrs.Any(a =>
                        string.Equals(a.AttributeClass?.Name, nameof(AddToInterfaceAttribute),
                            StringComparison.Ordinal)))
                {
                    memberContainsAttribute = true;
                }
            }

            switch (member)
            {
                case IMethodSymbol method when method.IsStatic || method.MethodKind != MethodKind.Ordinary:
                    continue;
                case IMethodSymbol method
                    when ObjectMethods.Contains(method.Name, StringComparer.Ordinal) && method.IsOverride:
                    continue;
                case IMethodSymbol when !settings.IncludeMethods && !memberContainsAttribute:
                    continue;
                case IMethodSymbol method:
                    methods.Add(ConvertMethod(method, ct));
                    break;
                case IPropertySymbol { IsStatic: true }:
                    continue;
                case IPropertySymbol when !settings.IncludeProperties && !memberContainsAttribute:
                    continue;
                case IPropertySymbol propertySymbol:
                    properties.Add(ConvertProperty(propertySymbol, ct));
                    break;
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
            implementPartial
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
        var parameters = method.Parameters.Select(parameter =>
                new ParameterToGenerate(parameter.Name, parameter.Type.ToDisplayString()))
            .ToArray();

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

        return new(property.Name, property.Type.ToDisplayString(), hasGetter, setPropertyType,
            property.GetDocumentationCommentXml(cancellationToken: ct));
    }
}
