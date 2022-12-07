namespace Roozie.AutoInterface;

internal static class InterfaceGenerator
{
    public static (string interfaceName, string sourceCode) Generate(InterfaceToGenerate toGenerate, string version)
    {
        const string spacer = "    ";

        var sb = new StringBuilder();
        foreach (var u in toGenerate.Usings
                     .Concat(new[] { "System.CodeDom.Compiler" }) // for GeneratedCodeAttribute
                     .Distinct(StringComparer.Ordinal)
                     .OrderBy(s => s, StringComparer.Ordinal))
        {
            sb.Append($"using {u};").AppendLine();
        }

        sb.Append(@"namespace ").AppendLine($"{toGenerate.Namespace};");
        sb.AppendLine("#nullable enable").AppendLine();

        AddXmlDoc(sb, toGenerate.XmlDoc, null);
        sb.AppendLine($@"[GeneratedCode(""{Shared.Namespace}"", ""{version}"")]");
        sb.AppendLine($@"public partial interface {toGenerate.Name}");
        sb.AppendLine("{");

        foreach (var property in toGenerate.Properties)
        {
            if (!property.HasGetter && property.SetType == null)
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

        return (toGenerate.Name, sb.ToString());
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
            if (s.Contains("member"))
            {
                continue;
            }

            sb.AppendLine($"{indent ?? string.Empty}///{s}");
        }
    }
}
