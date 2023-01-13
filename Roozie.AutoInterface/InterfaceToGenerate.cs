namespace Roozie.AutoInterface;

// File name must match type name
#pragma warning disable MA0048

internal record struct InterfaceToGenerate(
    string ClassName,
    string InterfaceName,
    string Namespace,
    IReadOnlyCollection<string> Usings,
    MethodToGenerate[] Methods,
    PropertyToGenerate[] Properties,
    string? XmlDoc,
    bool ImplementPartial
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
