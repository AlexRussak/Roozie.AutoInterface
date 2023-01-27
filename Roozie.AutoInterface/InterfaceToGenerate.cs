using Microsoft.CodeAnalysis;

namespace Roozie.AutoInterface;

// File name must match type name
#pragma warning disable MA0048

internal struct InterfaceToGenerate
{
    public InterfaceToGenerate(Accessibility accessibility, string className, string interfaceName, string ns,
        IReadOnlyCollection<string> usings, MethodToGenerate[] methods, PropertyToGenerate[] properties, string? xmlDoc,
        bool implementPartial, Location location)
        : this(accessibility, className, interfaceName, ns, usings, methods, properties, xmlDoc, implementPartial,
            location, null)
    {
    }

    private InterfaceToGenerate(Accessibility accessibility, string className, string interfaceName, string ns,
        IReadOnlyCollection<string> usings, MethodToGenerate[] methods, PropertyToGenerate[] properties, string? xmlDoc,
        bool implementPartial, Location location, ErrorType? errorType)
    {
        Accessibility = accessibility;
        ClassName = className;
        InterfaceName = interfaceName;
        Namespace = ns;
        Usings = usings;
        Methods = methods;
        Properties = properties;
        XmlDoc = xmlDoc;
        ImplementPartial = implementPartial;
        Location = location;
        ErrorType = errorType;
    }

    public static InterfaceToGenerate Error(
        Accessibility accessibility, string className, Location location, ErrorType errorType)
        => new(accessibility, className, string.Empty, string.Empty, Array.Empty<string>(),
            Array.Empty<MethodToGenerate>(), Array.Empty<PropertyToGenerate>(), null, false, location, errorType);

    public Accessibility Accessibility { get; }
    public string ClassName { get; }
    public string InterfaceName { get; }
    public string Namespace { get; }
    public IReadOnlyCollection<string> Usings { get; }
    public MethodToGenerate[] Methods { get; }
    public PropertyToGenerate[] Properties { get; }
    public string? XmlDoc { get; }
    public bool ImplementPartial { get; }
    public Location Location { get; }
    public ErrorType? ErrorType { get; }
}

internal readonly record struct MethodToGenerate(
    string Name,
    string ReturnType,
    ParameterToGenerate[] Parameters,
    string? XmlDoc
);

internal readonly record struct ParameterToGenerate(
    string Code
);

internal readonly record struct PropertyToGenerate(
    string Name,
    string Type,
    ParameterToGenerate[] Parameters,
    bool HasGetter,
    SetPropertyType? SetType,
    string? XmlDoc
);

internal enum SetPropertyType
{
    Set,
    Init
}

internal enum ErrorType
{
    StaticClass,
    InvalidAccessibility,
}
