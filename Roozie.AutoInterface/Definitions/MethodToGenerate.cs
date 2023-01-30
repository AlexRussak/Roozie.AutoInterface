namespace Roozie.AutoInterface.Definitions;

internal readonly record struct MethodToGenerate(
    string Name,
    string ReturnType,
    ParameterToGenerate[] Parameters,
    string? XmlDoc);
