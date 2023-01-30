namespace Roozie.AutoInterface;

internal static class InterfaceGenerator
{
    private const string Spacer = "    ";

    public static (string InterfaceName, string SourceCode) Generate(InterfaceToGenerate toGenerate, string version)
    {
        var sb = new StringBuilder(Shared.GetGeneratedFileComment(version));
        sb.AppendLine().AppendLine();

        foreach (var u in toGenerate.Usings.OrderBy(s => s, StringComparer.Ordinal))
        {
            sb.Append($"using {u};").AppendLine();
        }

        if (toGenerate.Usings.Count > 0)
        {
            sb.AppendLine();
        }

        sb.Append("namespace ").AppendLine($"{toGenerate.Namespace};").AppendLine();
        sb.AppendLine("#nullable enable").AppendLine();

        if (toGenerate.ImplementPartial)
        {
#pragma warning disable CA1308 // Need the lower case string
            sb.Append($"{toGenerate.Accessibility.ToString().ToLowerInvariant()} partial class ")
#pragma warning restore CA1308
                .AppendLine($"{toGenerate.ClassName} : {toGenerate.InterfaceName} {{}}")
                .AppendLine();
        }

        AddXmlDoc(sb, toGenerate.XmlDoc, null);
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
        var getString = property.HasGetter ? "get;" : string.Empty;
        var setString = property.SetType switch
        {
            SetPropertyType.Set => "set;",
            SetPropertyType.Init => "init;",
            _ => string.Empty
        };

        AddXmlDoc(sb, property.XmlDoc, Spacer);
        sb.Append($"{Spacer}{property.Type} {property.Name}");
        if (property.Parameters.Length > 0)
        {
            sb.Append('[');
            foreach (var parameter in property.Parameters)
            {
                sb.Append($"{parameter.Code}, ");
            }

            sb.Length -= 2;
            sb.Append(']');
        }

        sb.AppendLine($" {{{getString}{setString}}}").AppendLine();
        return sb.ToString();
    }

    private static string GenerateMethod(MethodToGenerate method)
    {
        var sb = new StringBuilder();
        AddXmlDoc(sb, method.XmlDoc, Spacer);
        sb.Append($"{Spacer}{method.ReturnType} {method.Name}(");
        if (method.Parameters.Length > 0)
        {
            foreach (var para in method.Parameters)
            {
                sb.Append(para.Code).Append(", ");
            }

            sb.Length -= 2;
        }

        sb.AppendLine(");").AppendLine();
        return sb.ToString();
    }

    private static void AddXmlDoc(StringBuilder sb, string? xmlDoc, string? indent)
    {
        if (string.IsNullOrEmpty(xmlDoc))
        {
            return;
        }

#pragma warning disable RS1035 // Need to use Environment.NewLine
        var split = xmlDoc!.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
#pragma warning restore RS1035
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
