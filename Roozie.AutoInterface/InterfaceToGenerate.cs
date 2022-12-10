namespace Roozie.AutoInterface;

internal record struct InterfaceToGenerate(
    string ClassName,
    string InterfaceName,
    string Namespace,
    IReadOnlyCollection<string> Usings,
    MethodToGenerate[] Methods,
    PropertyToGenerate[] Properties,
    string? XmlDoc,
    bool IsPartial
);

internal record struct MethodToGenerate(
    string Name,
    string ReturnType,
    ParameterToGenerate[] Parameters,
    string? XmlDoc
);

internal record struct ParameterToGenerate(
    string Name,
    string Type
);

internal record struct PropertyToGenerate(
    string Name,
    string Type,
    bool HasGetter,
    SetPropertyType? SetType,
    string? XmlDoc
);

public enum SetPropertyType
{
    Set,
    Init
}
