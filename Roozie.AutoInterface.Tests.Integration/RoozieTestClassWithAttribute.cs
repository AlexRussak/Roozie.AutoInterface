namespace Roozie.AutoInterface.Tests.Integration;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
#pragma warning disable CA1024
#pragma warning disable CA1044
#pragma warning disable CA1711
#pragma warning disable CA1822

/// <summary>
/// class documentation
/// </summary>
[AutoInterface(IncludeMethods = false, IncludeProperties = false)]
public class RoozieTestClassWithAttribute : IRoozieTestClassWithAttribute
{
    /// <summary>
    /// test doc on property
    /// </summary>
    [AddToInterface]
    public string PropertyNormalWithAttribute { get; set; } = "prop";

    /// <summary>
    /// test doc on another property
    /// </summary>
    [AddToInterface]
    public int PropertyGetWithAttribute { get; private set; }

    [AddToInterface]
    public int PropertyInitWithAttribute { get; init; }

    [AddToInterface]
    public string? PropPrivateGetWithAttribute { private get; set; }

    /// <summary>
    /// test doc on method
    /// </summary>
    [AddToInterface]
    public bool NoParamMethodWithAttribute() => true;

    /// <summary>
    /// test doc on method
    /// </summary>
    /// <param name="i">param doc</param>
    /// <returns>return doc</returns>
    [AddToInterface]
    public async Task<int> DoSomethingAsyncWithAttribute(int i) => await Task.FromResult(i + GetValue());

    [AddToInterface]
    public string? GetPropWithAttribute() => PropPrivateGetWithAttribute;

    private int GetValue()
    {
        PropertyGetWithAttribute = 1234;
        return 42;
    }
}
