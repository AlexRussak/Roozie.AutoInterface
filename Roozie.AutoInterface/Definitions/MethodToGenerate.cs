namespace Roozie.AutoInterface.Definitions;

internal readonly record struct MethodToGenerate(
    string Name,
    string ReturnType,
    string Parameters,
    string? XmlDoc,
    string? TypeParameters,
    string? TypeConstraints);
