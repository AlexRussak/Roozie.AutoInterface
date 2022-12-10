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
        GenerateAllMethods = true;
        GenerateAllProperties = true;
    }

    public {{Name}}(string? name, bool generateAllMethods = true, bool generateAllProperties = true)
    {
        Name = name;
        GenerateAllMethods = generateAllMethods;
        GenerateAllProperties = generateAllProperties;
    }

    public string? Name { get; set; }
    public bool GenerateAllMethods { get; set; } = true;
    public bool GenerateAllProperties { get; set; } = true;
}
""";
}
