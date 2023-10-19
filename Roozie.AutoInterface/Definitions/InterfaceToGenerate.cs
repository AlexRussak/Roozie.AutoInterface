using Microsoft.CodeAnalysis;

namespace Roozie.AutoInterface.Definitions;

internal readonly struct InterfaceToGenerate
{
    public InterfaceToGenerate(
        Accessibility accessibility,
        string className,
        string? classTypeParameters,
        string? classTypeConstraints,
        string interfaceName,
        string ns,
        string usings,
        MethodToGenerate[] methods,
        PropertyToGenerate[] properties,
        string? xmlDoc,
        bool implementPartial,
        Location location)
        : this(accessibility,
            className,
            classTypeParameters,
            classTypeConstraints,
            interfaceName,
            ns,
            usings,
            methods,
            properties,
            xmlDoc,
            implementPartial,
            location,
            null)
    {
    }

    private InterfaceToGenerate(
        Accessibility accessibility,
        string className,
        string? classTypeParameters,
        string? classTypeConstraints,
        string interfaceName,
        string ns,
        string usings,
        MethodToGenerate[] methods,
        PropertyToGenerate[] properties,
        string? xmlDoc,
        bool implementPartial,
        Location location,
        ErrorType? errorType)
    {
        Accessibility = accessibility;
        ClassName = className;
        ClassTypeParameters = classTypeParameters;
        ClassTypeConstraints = classTypeConstraints;
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

    public Accessibility Accessibility { get; }
    public string ClassName { get; }
    public string? ClassTypeParameters { get; }
    public string? ClassTypeConstraints { get; }
    public string InterfaceName { get; }
    public string Namespace { get; }
    public string Usings { get; }
    public MethodToGenerate[] Methods { get; }
    public PropertyToGenerate[] Properties { get; }
    public string? XmlDoc { get; }
    public bool ImplementPartial { get; }
    public Location Location { get; }
    public ErrorType? ErrorType { get; }

    public static InterfaceToGenerate Error(
        Accessibility accessibility,
        string className,
        Location location,
        ErrorType errorType)
    {
        return new(accessibility,
            className,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            Array.Empty<MethodToGenerate>(),
            Array.Empty<PropertyToGenerate>(),
            null,
            false,
            location,
            errorType);
    }
}
