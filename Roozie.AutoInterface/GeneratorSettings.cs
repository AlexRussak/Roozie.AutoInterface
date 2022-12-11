namespace Roozie.AutoInterface;

internal record struct GeneratorSettings(
    string? InterfaceName,
    bool IncludeMethods,
    bool IncludeProperties,
    bool ImplementOnPartial
);
