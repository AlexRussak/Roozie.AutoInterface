namespace Roozie.AutoInterface;

/// <summary>
/// Add this attribute to a class and an interface will be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoInterfaceAttribute : Attribute
{
    /// <summary>
    /// The name of the interface to generate.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// True to include all public methods in the interface, default is true.
    /// </summary>
    public bool IncludeMethods { get; set; } = true;

    /// <summary>
    /// True to include all public properties in the interface, default is true.
    /// </summary>
    public bool IncludeProperties { get; set; } = true;

    /// <summary>
    /// True to implement the interface when the class is partial, default is true.
    /// </summary>
    public bool ImplementOnPartial { get; set; } = true;
}
