using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roozie.AutoInterface;

[Generator(LanguageNames.CSharp)]
public class Generator : IIncrementalGenerator
{
    private static readonly string Version
        = typeof(Generator).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? typeof(Generator).Assembly.GetName().Version.ToString();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var interfacesToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
            Shared.FullNames.AutoInterfaceAttribute, static (node, _) => node is ClassDeclarationSyntax,
            static (syntaxContext, token) => Generate(syntaxContext, token)).Where(static s => s != null);

        context.RegisterSourceOutput(interfacesToGenerate, static (spc, i) => Execute(i!.Value, spc));
    }

    private static InterfaceToGenerate? Generate(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var attribute = context.Attributes.Single();
        return InterfaceExtractor.ProcessClass(attribute, context.TargetNode as ClassDeclarationSyntax,
            context.SemanticModel, token);
    }

    private static void Execute(InterfaceToGenerate interfaceToGenerate, SourceProductionContext context)
    {
        if (interfaceToGenerate.ErrorType != null)
        {
            context.ReportDiagnostic(CreateDiagnostic(interfaceToGenerate, interfaceToGenerate.ErrorType.Value));
            return;
        }

        var (interfaceName, source) = InterfaceGenerator.Generate(interfaceToGenerate, Version);
        context.AddSource($"{interfaceName}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static Diagnostic CreateDiagnostic(InterfaceToGenerate interfaceToGenerate, ErrorType type) =>
        type switch
        {
            ErrorType.StaticClass => Diagnostic.Create(
                new("RZAI001", "Static classes are not supported",
                    "'{0}' is a static class and cannot be used with AutoInterface", Shared.Namespace,
                    DiagnosticSeverity.Error, true), interfaceToGenerate.Location, interfaceToGenerate.ClassName),
            ErrorType.InvalidAccessibility => Diagnostic.Create(
                new("RZAI002", "Invalid accessibility",
                    "'{0}' is set to {1}. Classes must be either public or internal to be used with AutoInterface",
                    Shared.Namespace, DiagnosticSeverity.Error, true), interfaceToGenerate.Location,
                interfaceToGenerate.ClassName, interfaceToGenerate.Accessibility),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}
