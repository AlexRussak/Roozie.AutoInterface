namespace Roozie.AutoInterface;

internal static class InterfaceGenerator
{
    private const string Spacer = "    ";

    public static (string InterfaceName, string SourceCode) Generate(InterfaceToGenerate toGenerate, string version)
    {
        var sb = new StringBuilder(Helpers.GetGeneratedFileComment(version));
        sb.AppendLine().AppendLine();

        foreach (var u in toGenerate.Usings.OrderBy(s => s, StringComparer.Ordinal))
        {
            sb.Append($"using {u};").AppendLine();
        }

        if (toGenerate.Usings.Length > 0)
        {
            sb.AppendLine();
        }

        sb.Append("namespace ").AppendLine($"{toGenerate.Namespace};").AppendLine();
        sb.AppendLine("#nullable enable").AppendLine();

        if (toGenerate.ImplementPartial)
        {
            sb.Append($"{toGenerate.Accessibility.ToString().ToLowerInvariant()} partial class ")
                .AppendLine($"{toGenerate.ClassName} : {toGenerate.InterfaceName} {{}}")
                .AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(toGenerate.XmlDoc))
        {
            sb.AppendLine(TrimLineBreaks(toGenerate.XmlDoc!).TrimEnd());
        }

        sb.AppendLine($"public partial interface {toGenerate.InterfaceName}");
        sb.AppendLine("{");

        foreach (var property in toGenerate.Properties)
        {
            var propCode = GenerateProperty(property);
            if (propCode == null)
            {
                continue;
            }

            sb.Append(propCode);
        }

        foreach (var method in toGenerate.Methods)
        {
            var methodCode = GenerateMethod(method);
            sb.Append(methodCode);
        }

        // Close interface
        sb.Length--; // Remove last newline
        sb.AppendLine("}");

        return (toGenerate.InterfaceName, sb.ToString());
    }

    private static string? GenerateProperty(PropertyToGenerate property)
    {
        if (property is { HasGetter: false, SetType: null })
        {
            return null;
        }

        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(property.XmlDoc))
        {
            sb.AppendLine(TrimLineBreaks(property.XmlDoc!).TrimEnd());
        }

        sb.Append($"{Spacer}{property.Type.Trim()} {property.Name.Trim()}");
        if (!string.IsNullOrWhiteSpace(property.Parameters))
        {
            sb.Append(property.Parameters!.Trim());
        }

        var getString = property.HasGetter ? "get;" : string.Empty;
        var setString = property.SetType switch
        {
            SetPropertyType.Set => "set;",
            SetPropertyType.Init => "init;",
            _ => string.Empty
        };

        sb.Append(" { ");
        if (!string.IsNullOrWhiteSpace(getString))
        {
            sb.Append(getString).Append(' ');
        }

        if (!string.IsNullOrWhiteSpace(setString))
        {
            sb.Append(setString).Append(' ');
        }

        sb.AppendLine("}").AppendLine();
        return sb.ToString();
    }

    private static string GenerateMethod(MethodToGenerate method)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(method.XmlDoc))
        {
            sb.AppendLine(TrimLineBreaks(method.XmlDoc!).TrimEnd());
        }

        sb.Append($"{Spacer}{method.ReturnType.Trim()} {method.Name.Trim()}");
        if (method.TypeParameters != null)
        {
            sb.Append(method.TypeParameters.Trim());
        }

        sb.Append(method.Parameters.Trim());

        if (method.TypeConstraints != null)
        {
            sb.Append(' ').Append(method.TypeConstraints.Trim());
        }

        sb.AppendLine(";").AppendLine();
        return sb.ToString();
    }

    private static string TrimLineBreaks(string s)
    {
        return s.Trim('\r', '\n');
    }
}
