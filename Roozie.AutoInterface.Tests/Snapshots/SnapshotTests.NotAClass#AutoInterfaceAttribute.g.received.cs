//HintName: AutoInterfaceAttribute.g.cs

using System;
using System.CodeDom.Compiler;

namespace Roozie.AutoInterface;
#nullable enable

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
[GeneratedCode("Roozie.AutoInterface", "1.0.0.0")]
internal sealed class AutoInterfaceAttribute : Attribute
{
    public AutoInterfaceAttribute()
    {
        GenerateAllMethods = true;
        GenerateAllProperties = true;
    }

    public AutoInterfaceAttribute(string? name, bool generateAllMethods = true, bool generateAllProperties = true)
    {
        Name = name;
        GenerateAllMethods = generateAllMethods;
        GenerateAllProperties = generateAllProperties;
    }

    public string? Name { get; set; }
    public bool GenerateAllMethods { get; set; } = true;
    public bool GenerateAllProperties { get; set; } = true;
}
