namespace Roozie.AutoInterface.Definitions;

internal static class AddToInterfaceAttribute
{
    public const string Name = "AddToInterfaceAttribute";
    public const string FullName = $"{Shared.Namespace}.{Name}";

    public static string Code(string version) => @$"
using System;
using System.CodeDom.Compiler;

namespace {Shared.Namespace};
#nullable enable

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
[GeneratedCode(""{Shared.Namespace}"", ""{version}"")]
internal sealed class {Name} : Attribute
{{
}}
";
}
