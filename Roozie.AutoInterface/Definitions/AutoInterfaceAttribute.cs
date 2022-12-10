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

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal sealed class {{Name}} : Attribute
{
    public {{Name}}()
    {
        IncludeMethods = true;
        IncludeProperties = true;
    }

    public {{Name}}(string? name, bool includeMethods = true, bool includeProperties = true)
    {
        Name = name;
        IncludeMethods = includeMethods;
        IncludeProperties = includeProperties;
    }

    public string? Name { get; set; }
    public bool IncludeMethods { get; set; } = true;
    public bool IncludeProperties { get; set; } = true;
}
""";
}
