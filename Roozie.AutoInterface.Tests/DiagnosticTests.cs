namespace Roozie.AutoInterface.Tests;

[UsesVerify]
public class DiagnosticTests
{
    [Fact]
    public Task StaticClass()
    {
        const string source = $$"""
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public static class {{nameof(StaticClass)}}
{
    public Guid Property { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task PrivateClass()
    {
        const string source = $$"""
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
private class {{nameof(PrivateClass)}}
{
    private Guid Property { get; set; } = Guid.NewGuid();
    private string TestMethod(string input) => input;
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task ProtectedClass()
    {
        const string source = $$"""
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
protected class {{nameof(ProtectedClass)}}
{
    protected Guid Property { get; set; } = Guid.NewGuid();
    protected string TestMethod(string input) => input;
}
""";
        return TestHelper.Verify(source);
    }
}
