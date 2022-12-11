namespace Roozie.AutoInterface;

internal static class InterfaceGenerator
{
    public static (string interfaceName, string sourceCode) Generate(InterfaceToGenerate toGenerate, string version)
    {
        const string spacer = "    ";

        var sb = new StringBuilder(Shared.GetGeneratedFileComment(version));
        sb.AppendLine();

        foreach (var u in toGenerate.Usings.OrderBy(s => s, StringComparer.Ordinal))
        {
            sb.Append($"using {u};").AppendLine();
        }

        sb.Append("namespace ").AppendLine($"{toGenerate.Namespace};");
        sb.AppendLine("#nullable enable").AppendLine();

        if (toGenerate.ImplementPartial)
        {
            sb.AppendLine($"public partial class {toGenerate.ClassName} : {toGenerate.InterfaceName} {{}}")
                .AppendLine();
        }

        AddXmlDoc(sb, toGenerate.XmlDoc, null);
        sb.AppendLine($"public partial interface {toGenerate.InterfaceName}");
        sb.AppendLine("{");

        foreach (var property in toGenerate.Properties)
        {
            if (property is { HasGetter: false, SetType: null })
            {
                continue;
            }

            var getString = property.HasGetter ? "get;" : string.Empty;
            var setString = property.SetType switch
            {
                SetPropertyType.Set => "set;",
                SetPropertyType.Init => "init;",
                _ => string.Empty
            };

            AddXmlDoc(sb, property.XmlDoc, spacer);
            sb.AppendLine($"{spacer}{property.Type} {property.Name} {{{getString}{setString}}}").AppendLine();
        }

        foreach (var method in toGenerate.Methods)
        {
            AddXmlDoc(sb, method.XmlDoc, spacer);
            sb.Append($"{spacer}{method.ReturnType} {method.Name}(");
            if (method.Parameters.Length > 0)
            {
                foreach (var (parameterName, parameterType) in method.Parameters)
                {
                    sb.Append(parameterType).Append(' ').Append(parameterName).Append(", ");
                }

                sb.Length -= 2;
            }

            sb.AppendLine(");").AppendLine();
        }

        // Close interface
        sb.Length--; // Remove last newline
        sb.AppendLine("}");

        return (toGenerate.InterfaceName, sb.ToString());
    }

    private static void AddXmlDoc(StringBuilder sb, string? xmlDoc, string? indent)
    {
        if (string.IsNullOrEmpty(xmlDoc))
        {
            return;
        }

        var split = xmlDoc!.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var s in split)
        {
            // The xml docs are wrapped in <member> tags, so we need to remove them
            // <member name="T:Roozie.AutoInterface.Tests.TestClass">
            // </member>
            if (s.StartsWith("<member ", StringComparison.Ordinal) ||
                string.Equals(s, "</member>", StringComparison.Ordinal))
            {
                continue;
            }

            sb.AppendLine($"{indent ?? string.Empty}///{RemoveLeadingSpaces(s, 3)}");
        }
    }

    private static string RemoveLeadingSpaces(string s, int removeCount)
    {
        var index = 0;
        while (index < s.Length && index < removeCount && s[index] == ' ')
        {
            index++;
        }

        return s.Substring(index);
    }
}
