using System.Drawing;

namespace Roozie.AutoInterface.Tests.Integration;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
#pragma warning disable CA1024
#pragma warning disable CA1044
#pragma warning disable CA1822

/// <summary>
/// class documentation
/// </summary>
[AutoInterface]
internal partial class RoozieTestClassPartial
{
    /// <summary>
    /// test doc on method
    /// </summary>
    public bool NoParamMethod() => true;

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

    public string? GetProp(string? blah = null) => PropPrivateGet;
    public string? GetProp2(TimeSpan? blah = null) => PropPrivateGet;
    public int[] GetArray(params int[] i) => i;

    public string EnumTest(KnownColor c = KnownColor.Red) => c.ToString();


    private int GetValue()
    {
        PropertyGet = 1234;
        return 42;
    }

    /// <summary>
    /// test doc on property
    /// </summary>
    public string PropertyNormal { get; set; } = "prop";

    /// <summary>
    /// test doc on another property
    /// </summary>
    public int PropertyGet { get; private set; }

    public int PropertyInit { get; init; }

    public string? PropPrivateGet { private get; set; }
}
