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

        var interfacesToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
                Shared.FullNames.AutoInterfaceAttribute,
                (node, _) => node is ClassDeclarationSyntax,
                (syntaxContext, token) => Generate(syntaxContext, token))
            .Where(static s => s != null);

        context.RegisterSourceOutput(interfacesToGenerate,
            (spc, i) => Execute(i!.Value, spc, version));
    }

    private static InterfaceToGenerate? Generate(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var attribute = context.Attributes.Single();
        return InterfaceExtractor.ProcessClass(attribute, context.TargetNode as ClassDeclarationSyntax,
            context.SemanticModel, token);
    }

    private static void Execute(InterfaceToGenerate interfaceToGenerate, SourceProductionContext context,
        string version)
    {
        var (interfaceName, source) = InterfaceGenerator.Generate(interfaceToGenerate, version);
        context.AddSource($"{interfaceName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
