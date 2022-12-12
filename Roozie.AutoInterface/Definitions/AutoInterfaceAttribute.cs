namespace Roozie.AutoInterface.Definitions;

internal static class AutoInterfaceAttribute
{
    public const string Name = "AutoInterfaceAttribute";
    public const string FullName = $"{Shared.Namespace}.{Name}";

    public static string Code(string version) => $$"""
{{Shared.GetGeneratedFileComment(version)}}
using System;

namespace {{Shared.Namespace}};
#nullable enable

/// <summary>
/// Add this attribute to a class and an interface will be generated.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal sealed class {{Name}} : Attribute
{
    public {{Name}}()
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
""";
}
