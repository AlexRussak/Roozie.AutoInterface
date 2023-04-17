using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roozie.AutoInterface;

internal static class InterfaceExtractor
{
    private static readonly string[] ObjectMethods = { nameof(ToString), nameof(GetHashCode), nameof(Equals), };

    public static InterfaceToGenerate? ProcessClass(
        AttributeData attributeData,
        ClassDeclarationSyntax? classDeclarationSyntax,
        SemanticModel semanticModel,
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

        if (classSymbol.IsStatic)
        {
            return InterfaceToGenerate.Error(classSymbol.DeclaredAccessibility,
                classSymbol.Name,
                classDeclarationSyntax.GetLocation(),
                ErrorType.StaticClass);
        }

        if (classSymbol.DeclaredAccessibility is not Accessibility.Public and not Accessibility.Internal)
        {
            return InterfaceToGenerate.Error(classSymbol.DeclaredAccessibility,
                classSymbol.Name,
                classDeclarationSyntax.GetLocation(),
                ErrorType.InvalidAccessibility);
        }

        var settings = GetSettings(attributeData);
        var methods = new List<MethodToGenerate>();
        var properties = new List<PropertyToGenerate>();
        foreach (var memberSyntax in classDeclarationSyntax.Members)
        {
            if (memberSyntax.Modifiers.Any(SyntaxKind.StaticKeyword) ||
                memberSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword) ||
                memberSyntax.Modifiers.Any(SyntaxKind.ProtectedKeyword) ||
                memberSyntax.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                continue;
            }

            var memberSymbol = semanticModel.GetDeclaredSymbol(memberSyntax, ct);
            if (memberSymbol == null)
            {
                continue;
            }

            var (method, property) = ConvertMember(memberSyntax, memberSymbol, settings);
            if (method != null)
            {
                methods.Add(method.Value);
            }

            if (property != null)
            {
                properties.Add(property.Value);
            }
        }

        var root = classDeclarationSyntax.SyntaxTree.GetCompilationUnitRoot(ct);
        var usings = root.Usings.Select(static u => u.Name.ToString())
            .Where(static u =>
                !string.IsNullOrWhiteSpace(u) && !string.Equals(u, Helpers.Namespace, StringComparison.Ordinal))
            .OrderBy(static u => u, StringComparer.Ordinal)
            .ToArray();

        var classDoc = classDeclarationSyntax.HasLeadingTrivia
            ? classDeclarationSyntax.GetLeadingTrivia().ToFullString()
            : null;

        var ns = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : classSymbol.ContainingNamespace.ToString();
        var implementPartial = classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) &&
                               settings.ImplementOnPartial;
        return new(classSymbol.DeclaredAccessibility,
            classSymbol.Name,
            settings.InterfaceName ?? "I" + classSymbol.Name,
            ns,
            usings,
            methods.ToArray(),
            properties.ToArray(),
            classDoc,
            implementPartial,
            classDeclarationSyntax.GetLocation());
    }

    private static (MethodToGenerate? Method, PropertyToGenerate? Property) ConvertMember(
        MemberDeclarationSyntax memberSyntax,
        ISymbol symbol,
        GeneratorSettings settings)
    {
        var memberContainsAttribute = false;
        var memberAttrs = symbol.GetAttributes();
        if (memberAttrs.Any(static a =>
                string.Equals(a.AttributeClass?.Name, nameof(AddToInterfaceAttribute), StringComparison.Ordinal)))
        {
            memberContainsAttribute = true;
        }

        if (symbol is IMethodSymbol methodSymbol && (memberContainsAttribute || settings.IncludeMethods))
        {
            if (ObjectMethods.Contains(methodSymbol.Name, StringComparer.Ordinal) ||
                methodSymbol.MethodKind != MethodKind.Ordinary)
            {
                return (null, null);
            }

            var methodSyntax = (MethodDeclarationSyntax)memberSyntax;
            return (ConvertMethod(methodSyntax), null);
        }

        if (symbol is IPropertySymbol propertySymbol && (memberContainsAttribute || settings.IncludeProperties))
        {
            switch (memberSyntax)
            {
                case PropertyDeclarationSyntax propertySyntax:
                    return (null, ConvertProperty(propertySymbol, propertySyntax));
                case IndexerDeclarationSyntax indexerSyntax:
                    return (null, ConvertIndexer(propertySymbol, indexerSyntax));
            }
        }

        return (null, null);
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

            if (string.Equals(key, nameof(AutoInterfaceAttribute.Name), StringComparison.Ordinal) &&
                value.Value?.ToString() is { } s &&
                !string.IsNullOrWhiteSpace(s))
            {
                interfaceName = s;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.IncludeMethods), StringComparison.Ordinal) &&
                value.Value is bool methodsFlag)
            {
                includeMethods = methodsFlag;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.IncludeProperties), StringComparison.Ordinal) &&
                value.Value is bool propertiesFlag)
            {
                includeProperties = propertiesFlag;
            }

            if (string.Equals(key, nameof(AutoInterfaceAttribute.ImplementOnPartial), StringComparison.Ordinal) &&
                value.Value is bool partialFlag)
            {
                implementOnPartial = partialFlag;
            }
        }

        return new(interfaceName, includeMethods ?? true, includeProperties ?? true, implementOnPartial ?? true);
    }

    private static MethodToGenerate ConvertMethod(MethodDeclarationSyntax syntax)
    {
        var typeConstraints = syntax.ConstraintClauses.Count > 0 ? syntax.ConstraintClauses.ToFullString() : null;
        var trivia = syntax.HasLeadingTrivia ? syntax.GetLeadingTrivia().ToFullString() : null;

        return new(syntax.Identifier.ToFullString(),
            syntax.ReturnType.ToFullString(),
            syntax.ParameterList.ToFullString(),
            trivia,
            syntax.TypeParameterList?.ToFullString(),
            typeConstraints);
    }

    private static PropertyToGenerate ConvertProperty(IPropertySymbol symbol, PropertyDeclarationSyntax syntax)
    {
        if (symbol.IsIndexer)
        {
            throw new ArgumentException("Property cannot be an indexer", nameof(symbol));
        }

        var (hasGetter, setPropertyType) = GetPropertyAccessors(symbol);
        var trivia = syntax.HasLeadingTrivia ? syntax.GetLeadingTrivia().ToFullString() : null;

        return new(syntax.Identifier.ToFullString(),
            syntax.Type.ToFullString(),
            null,
            hasGetter,
            setPropertyType,
            trivia);
    }

    private static PropertyToGenerate ConvertIndexer(IPropertySymbol symbol, IndexerDeclarationSyntax syntax)
    {
        if (!symbol.IsIndexer)
        {
            throw new ArgumentException("Property is not an indexer", nameof(symbol));
        }

        var (hasGetter, setPropertyType) = GetPropertyAccessors(symbol);
        var trivia = syntax.HasLeadingTrivia ? syntax.GetLeadingTrivia().ToFullString() : null;

        return new("this",
            syntax.Type.ToFullString(),
            syntax.ParameterList.ToFullString(),
            hasGetter,
            setPropertyType,
            trivia);
    }

    private static (bool HasGetter, SetPropertyType? SetPropertyType) GetPropertyAccessors(IPropertySymbol symbol)
    {
        var hasGetter = symbol.GetMethod is { DeclaredAccessibility: Accessibility.Public };

        SetPropertyType? setPropertyType = null;
        if (symbol.SetMethod is { DeclaredAccessibility: Accessibility.Public })
        {
            setPropertyType = symbol.SetMethod.IsInitOnly ? SetPropertyType.Init : SetPropertyType.Set;
        }

        return (hasGetter, setPropertyType);
    }
}
