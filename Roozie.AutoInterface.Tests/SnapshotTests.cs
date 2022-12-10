namespace Roozie.AutoInterface.Tests;

[UsesVerify]
public class SnapshotTests
{
    [Fact]
    public Task Simple()
    {
        const string source = $$"""
using System;
using Roozie.AutoInterface;
using System.Text;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
[Serializable]
public class {{nameof(Simple)}}
{
    public Guid Property { get; set; } = Guid.NewGuid();
    public string? PropertyInvalid { private get; private set; }
    public int PropertyInit { get; init; }
    public string? PropertyPrivateSet { get; private set; }

    public string TestMethod(string input) => input;
    private string TestMethodPrivate(string input) => input;
    internal string TestMethodInternal(string input) => input;

    public override bool Equals(object? obj) => base.Equals(obj);
    public override string ToString() => base.ToString();
    public override int GetHashCode() => base.GetHashCode();
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task InterfaceName()
    {
        const string source = $$"""
using System;
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface(Name = "ADifferentInterfaceName")]
public partial class {{nameof(InterfaceName)}}
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task WithoutAttribute()
    {
        const string source = $$"""
using System;
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

public class {{nameof(WithoutAttribute)}}
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TestMethod(string input) => input;
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task AddToInterface_Attribute()
    {
        const string source = $$"""
using System;
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

[AutoInterface(GenerateAllMethods = false, GenerateAllProperties = false)]
public class {{nameof(AddToInterface_Attribute)}}
{
    public string? Test { get; private set; }
    public int TestMethod(int input) => input;

    [AddToInterface]
    public string? TestPropWithAttribute { get; set; }

    [AddToInterface]
    public string? TestPropWithAttributePrivateSet { get; private set; }

    [AddToInterface]
    public string? TestPropWithAttributeInit { get; init; }

    [AddToInterface]
    public int TestMethodWithAttribute(int input) => input;
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task XmlDocComments()
    {
        const string source = $$"""
using System;
using Roozie.AutoInterface;

namespace Roozie.AutoInterface.Tests;

/// <summary>
/// Test class
/// </summary>
/// <remarks>
/// Test class remarks
/// </remarks>
[AutoInterface]
public class {{nameof(XmlDocComments)}}
{
    /// <summary>
    /// Test property
    /// </summary>
    /// <remarks>
    /// Test property remarks
    /// </remarks>
    public string? Test { get; private set; }

    /// <summary>
    /// Test method <see cref="TestClass"/>
    /// </summary>
    /// <remarks>
    /// Test method remarks
    /// </remarks>
    /// <param name="input">Test input</param>
    /// <returns>Test output</returns>
    public int TestMethod(int input) => input;
}
""";
        return TestHelper.Verify(source);
    }
}
