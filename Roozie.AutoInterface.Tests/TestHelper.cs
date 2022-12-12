using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roozie.AutoInterface.Tests;

public static class TestHelper
{
    public static Task Verify(string source, [CallerMemberName] string memberName = "")
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references for assemblies we require
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        // Create a Roslyn compilation for the syntax tree
        var compilation = CSharpCompilation.Create(
            "Tests",
            new[] { syntaxTree },
            references);

        // Create an instance of our AutoInterface incremental source generator
        var generator = new Generator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        // Use verify to snapshot test the source generator output
        return Verifier
            .Verify(driver)
            .UseDirectory(Path.Combine("Snapshots", memberName));
    }
}
