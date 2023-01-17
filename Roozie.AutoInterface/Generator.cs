using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roozie.AutoInterface;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var assembly = typeof(Generator).Assembly;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                      ?? assembly.GetName().Version.ToString();

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

        // Convert each class to an interface to generate
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
        var classAttribute = compilation.GetTypeByMetadataName(Shared.FullNames.AutoInterfaceAttribute);
        if (classAttribute == null)
        {
            yield break;
        }

        foreach (var classDeclarationSyntax in classes)
        {
            var result = InterfaceExtractor.ProcessClass(classAttribute, classDeclarationSyntax, compilation, ct);
            if (result != null)
            {
                yield return result.Value;
            }
        }
    }

    // Only process nodes that are classes with at least one attribute
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
                if (string.Equals(fullName, Shared.FullNames.AutoInterfaceAttribute, StringComparison.Ordinal))
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }
}
