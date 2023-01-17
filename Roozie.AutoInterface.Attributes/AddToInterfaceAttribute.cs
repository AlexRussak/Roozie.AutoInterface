namespace Roozie.AutoInterface;

/// <summary>
/// Add this attribute to mark a member that should be added to the generated interface.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
public sealed class AddToInterfaceAttribute : Attribute
{
}
