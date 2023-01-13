namespace Roozie.AutoInterface;

internal readonly record struct InterfaceToGenerate(
    string ClassName,
    string InterfaceName,
    string Namespace,
    IReadOnlyCollection<string> Usings,
    MethodToGenerate[] Methods,
    PropertyToGenerate[] Properties,
    string? XmlDoc,
    bool ImplementPartial
);

internal readonly record struct MethodToGenerate(
    string Name,
    string ReturnType,
    ParameterToGenerate[] Parameters,
    string? XmlDoc
)
{
    public bool Equals(MethodToGenerate other)
    {
        // Compare the name and the parameter types
        if (!string.Equals(Name, other.Name, StringComparison.Ordinal) ||
            Parameters.Length != other.Parameters.Length)
        {
            return false;
        }

        for (var i = 0; i < Parameters.Length; i++)
        {
            if (!string.Equals(Parameters[i].Type, other.Parameters[i].Type, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = StringComparer.Ordinal.GetHashCode(Name);
            for (var i = 0; i < Parameters.Length; i++)
            {
                hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(Parameters[i].Type);
            }

            return hashCode;
        }
    }
}

internal readonly record struct ParameterToGenerate(
    string Name,
    string Type
);

internal readonly record struct PropertyToGenerate(
    string Name,
    string Type,
    bool HasGetter,
    SetPropertyType? SetType,
    string? XmlDoc
)
{
    public bool Equals(PropertyToGenerate other) => string.Equals(Name, other.Name, StringComparison.Ordinal);

    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Name);
}

public enum SetPropertyType
{
    Set,
    Init
}
