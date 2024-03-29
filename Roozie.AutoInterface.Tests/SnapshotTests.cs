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
using FS = System.IO.FileStream;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
[Serializable]
public class {{nameof(Simple)}}
{
    public Guid Property { get; set; } = Guid.NewGuid();
    public string? PropertyInvalid { private get; private set; }
    public int PropertyInit { get; init; }
    public string? PropertyPrivateSet { get; private set; }

    public string TestMethod(string input = "test") => input;
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

[AutoInterface(Name = "ADifferentInterfaceName", ImplementOnPartial = false)]
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

[AutoInterface(IncludeMethods = false, IncludeProperties = false)]
public class {{nameof(AddToInterface_Attribute)}}
{
    public string? TestPropWithoutAttribute { get; private set; }
    public int TestMethodWithoutAttribute(int input) => input;

    [AddToInterface]
    public string? TestPropWithAttribute { get; set; }

    [AddToInterface]
    public string TestPropWithAttributePrivateSet { get; private set; }

    [AddToInterface]
    public Guid? TestPropWithAttributeInit { get; init; }

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
namespace Roozie.AutoInterface.Tests;

/// <summary>
/// Test class
/// </summary>
/// <remarks>
/// Test class remarks
/// </remarks>
[AutoInterface]
internal partial class {{nameof(XmlDocComments)}}
{
    /// <summary>
    /// Test property
    /// </summary>
    /// <remarks>
    /// Test property remarks
    /// </remarks>
    public string? Test { get; private set; }

    /// <summary>
    /// Test method <see cref="{{nameof(XmlDocComments)}}"/>
    /// </summary>
    /// <remarks>
    /// Test method remarks
    /// </remarks>
    /// <param name="input">Test input</param>
    /// <returns>Test output</returns>
    public int TestMethod(int input) => input;

    /// <summary>
    /// test doc on method
    /// </summary>
    /// <param name="i">param doc</param>
    /// <param name="b">b</param>
    /// <param name="s">s</param>
    /// <param name="c">c</param>
    /// <param name="l1">l1</param>
    /// <param name="dm">dm</param>
    /// <param name="dd">dd</param>
    /// <param name="f">f</param>
    /// <param name="l2">l2</param>
    /// <param name="ct">ct</param>
    /// <returns>return doc</returns>
    public async Task<int> DoSomethingAsync(int i = 123456, bool b = false, string s = "s", char c = 'c',
        long l1 = 111L, decimal dm = 1234.5m, double dd = 5678.901d, float f = 98765.04f, long l2 = long.MaxValue,
        CancellationToken ct = default) =>
        await Task.FromResult(i + GetValue());
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task Indexers()
    {
        const string source = $$"""
using Roozie.AutoInterface;
using System.Drawing;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
internal class {{nameof(Indexers)}}
{
    public int this[int index]
    {
        get { return 0; }
        set { }
    }

    public int this[string v1]
    {
        get { return 0; }
        set { }
    }

    public int this[string v1 = "", decimal v2 = 1234.1234m, KnownColor color = KnownColor.Aqua]
    {
        get { return 0; }
        set { }
    }
}
""";
        return TestHelper.Verify(source);
    }

    [Fact]
    public Task GenericMethods()
    {
        const string source = $$"""
using Roozie.AutoInterface;
using System.Drawing;

namespace Roozie.AutoInterface.Tests;

[AutoInterface]
internal class {{nameof(GenericMethods)}}
{
    /// <summary>
    /// This is a test comment for <see cref="System.Drawing.Color"/>
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="T">doc</typeparam>
    /// <returns></returns>
    public T TestMethod<T>(T input) => input;

    public T3? TestMethod2<T1, T2, T3>(T1 input1, T2 input2) => default;

    public T TestMethod3<T>(T input) where T : class => input;
}
""";
        return TestHelper.Verify(source);
    }
}
