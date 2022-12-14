//HintName: AutoInterfaceAttribute.g.cs
// This file was generated by Roozie.AutoInterface v0.1.1-beta
using System;

namespace Roozie.AutoInterface;
#nullable enable

/// <summary>
/// Add this attribute to a class and an interface will be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal sealed class AutoInterfaceAttribute : Attribute
{
    public AutoInterfaceAttribute()
    {
        IncludeMethods = true;
        IncludeProperties = true;
        ImplementOnPartial = true;
    }

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
