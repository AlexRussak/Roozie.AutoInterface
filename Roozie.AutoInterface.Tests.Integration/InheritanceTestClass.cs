namespace Roozie.AutoInterface.Tests.Integration;

[AutoInterface]
public class InheritanceTestClass : InheritanceTestClassBase, IInheritanceTestClass
{
    public override string? TestString { get; set; }

    public new int TestMethod() => int.Parse(TestString!, CultureInfo.InvariantCulture);

    public new string TestMethod(int abc) => abc.ToString(CultureInfo.InvariantCulture) + TestString;

    public Guid NewProperty { get; set; }
    public Guid? NewMethod() => NewProperty;
}
