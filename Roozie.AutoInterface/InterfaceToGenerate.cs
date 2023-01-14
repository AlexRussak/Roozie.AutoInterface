namespace Roozie.AutoInterface;

// File name must match type name
#pragma warning disable MA0048

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
);

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
);

internal enum SetPropertyType
{
    Set,
    Init
}
