namespace Roozie.AutoInterface;

internal record struct InterfaceToGenerate(
    string Name,
    string Namespace,
    MethodToGenerate[] Methods,
    PropertyToGenerate[] Properties,
    string? XmlDoc
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
