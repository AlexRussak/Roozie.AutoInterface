namespace Roozie.AutoInterface.Tests.Integration;

public abstract class InheritanceTestClassBase
{
    public virtual string? TestString { get; set; }
    public virtual string? TestMethod() => TestString;

    public virtual int TestMethod(int i) => i;
}
