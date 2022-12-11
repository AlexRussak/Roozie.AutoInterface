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
public partial class RoozieTestClassPartial
{
    /// <summary>
    /// test doc on method
    /// </summary>
    public bool NoParamMethod() => true;

    /// <summary>
    /// test doc on method
    /// </summary>
    /// <param name="i">param doc</param>
    /// <returns>return doc</returns>
    public async Task<int> DoSomethingAsync(int i) => await Task.FromResult(i + GetValue());

    public string? GetProp() => PropPrivateGet;

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
