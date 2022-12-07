namespace Roozie.AutoInterface.Tests;

[UsesVerify]
public class SnapshotTests
{
    [Fact]
    public Task GeneratesAutoInterfaceCorrectly()
    {
        const string source = @"
using System;
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
public class TestClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
}
