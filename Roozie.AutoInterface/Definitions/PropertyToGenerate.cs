namespace Roozie.AutoInterface.Definitions;

internal readonly record struct PropertyToGenerate(
    string Name,
    string Type,
    ParameterToGenerate[] Parameters,
    bool HasGetter,
    SetPropertyType? SetType,
    string? XmlDoc);
