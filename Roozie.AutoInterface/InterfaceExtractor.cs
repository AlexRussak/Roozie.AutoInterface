using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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

    public static InterfaceToGenerate? ProcessClass(INamedTypeSymbol classAttribute,
        ClassDeclarationSyntax? classDeclarationSyntax, Compilation compilation,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (classDeclarationSyntax == null)
        {
            return null;
        }

        var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax, ct) is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        string? interfaceName = null;
        var generateAllMethods = true;
        var generateAllProperties = true;
        var attributes = classSymbol.GetAttributes();
        foreach (var attributeData in attributes)
        {
            if (!classAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
            {
                // This isn't the correct attribute
                continue;
            }

            (interfaceName, generateAllMethods, generateAllProperties) = GetAttributeSettings(attributeData);
            break;
        }

        var methods = new List<MethodToGenerate>();
        var properties = new List<PropertyToGenerate>();
        foreach (var member in classSymbol.GetMembers())
        {
            if (member.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            var memberContainsAttribute = false;
            if (generateAllMethods && generateAllProperties)
            {
                memberContainsAttribute = true;
            }
            else
            {
                var memberAttrs = member.GetAttributes();
                if (memberAttrs.Any(a =>
                        string.Equals(a.AttributeClass?.Name, AddToInterfaceAttribute.Name, StringComparison.Ordinal)))
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
                case IMethodSymbol when !generateAllMethods && !memberContainsAttribute:
                    continue;
                case IMethodSymbol method:
                    methods.Add(ConvertMethod(method, ct));
                    break;
                case IPropertySymbol when !generateAllProperties && !memberContainsAttribute:
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
            .Distinct(StringComparer.Ordinal)
            .OrderBy(u => u)
            .ToImmutableArray();
        var classDoc = classSymbol.GetDocumentationCommentXml(cancellationToken: ct);
        return new(
            interfaceName ?? "I" + classSymbol.Name,
            classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToString(),
            usings,
            methods.ToArray(),
            properties.ToArray(),
            classDoc);
    }

    private static (string? interfaceName, bool generateAllMethods, bool generateAllProperties)
        GetAttributeSettings(AttributeData attributeData)
    {
        string? interfaceName = null;
        bool? generateAllMethods = null;
        bool? generateAllProperties = null;

        foreach (var kvp in attributeData.NamedArguments)
        {
            if (string.Equals(kvp.Key, "Name", StringComparison.Ordinal)
                && kvp.Value.Value?.ToString() is { } s
                && !string.IsNullOrWhiteSpace(s))
            {
                interfaceName = s;
            }

            if (string.Equals(kvp.Key, "GenerateAllMethods", StringComparison.Ordinal)
                && kvp.Value.Value is bool methodsFlag)
            {
                generateAllMethods = methodsFlag;
            }

            if (string.Equals(kvp.Key, "GenerateAllProperties", StringComparison.Ordinal)
                && kvp.Value.Value is bool propertiesFlag)
            {
                generateAllProperties = propertiesFlag;
            }
        }

        return (interfaceName, generateAllMethods ?? true, generateAllProperties ?? true);
    }

    private static MethodToGenerate ConvertMethod(IMethodSymbol method, CancellationToken ct) =>
        new(method.Name, method.ReturnType.ToDisplayString(),
            method.Parameters.Select(parameter =>
                    new ParameterToGenerate(parameter.Name, parameter.Type.ToDisplayString()))
                .ToArray(),
            method.GetDocumentationCommentXml(cancellationToken: ct));

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
