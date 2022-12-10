namespace Roozie.AutoInterface.Definitions;

internal static class Shared
{
    public const string Namespace = $"{nameof(Roozie)}.{nameof(AutoInterface)}";

    public static string GetGeneratedFileComment(string version) =>
        $@"// This file was generated by {Namespace} v{version}";
}
