namespace Roozie.AutoInterface.Definitions;

internal static class AddToInterfaceAttribute
{
    public const string Name = "AddToInterfaceAttribute";
    public const string FullName = $"{Shared.Namespace}.{Name}";

    public static string Code(string version) => $$"""
{{Shared.GetGeneratedFileComment(version)}}
using System;

namespace {{Shared.Namespace}};
#nullable enable

/// <summary>
/// Add this attribute to mark a member that should be added to the generated interface.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal sealed class {{Name}} : Attribute
{
}
""";
}
