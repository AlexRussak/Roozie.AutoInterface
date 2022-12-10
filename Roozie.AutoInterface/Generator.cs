using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roozie.AutoInterface;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var version = typeof(Generator).Assembly.GetName().Version.ToString();
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{AutoInterfaceAttribute.Name}.g.cs",
            SourceText.From(AutoInterfaceAttribute.Code(version), Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{AddToInterfaceAttribute.Name}.g.cs",
            SourceText.From(AddToInterfaceAttribute.Code(version), Encoding.UTF8)));

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => IsSyntaxTargetForGeneration(s),
                static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static c => c is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
        {
            var (compilation, classDeclarationSyntaxes) = source;
            Execute(compilation, classDeclarationSyntaxes, spc, version);
        });
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax?> classes,
        SourceProductionContext context, string version)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }

        // Convert each to an interface to generate
        var interfacesToGenerate = GetTypesToGenerate(compilation, classes.Distinct(),
            context.CancellationToken);

        foreach (var interfaceToGenerate in interfacesToGenerate)
        {
            var (interfaceName, source) = InterfaceGenerator.Generate(interfaceToGenerate, version);
            context.AddSource($"{interfaceName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static IEnumerable<InterfaceToGenerate> GetTypesToGenerate(Compilation compilation,
        IEnumerable<ClassDeclarationSyntax?> classes, CancellationToken ct)
    {
        // Check for the attribute
        var classAttribute = compilation.GetTypeByMetadataName(AutoInterfaceAttribute.FullName);
        if (classAttribute != null)
        {
            foreach (var classDeclarationSyntax in classes)
            {
                var result = InterfaceExtractor.ProcessClass(classAttribute, classDeclarationSyntax, compilation, ct);
                if (result != null)
                {
                    yield return result.Value;
                }
            }
        }
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // loop through all the attributes on the class
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the [AutoInterface] attribute?
                if (string.Equals(fullName, AutoInterfaceAttribute.FullName, StringComparison.Ordinal))
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }
}
