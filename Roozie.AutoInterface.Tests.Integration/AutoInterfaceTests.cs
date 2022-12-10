namespace Roozie.AutoInterface.Tests.Integration;

public class AutoInterfaceTests
{
    [Fact]
    public async Task RoozieTestClass_VerifyInterface()
    {
        IRoozieTestClass target = new RoozieTestClass();

        target.NoParamMethod().Should().BeTrue();
        (await target.DoSomethingAsync(42)).Should().Be(84);
        target.PropertyNormal.Should().Be("prop");
        target.PropertyGet.Should().Be(1234);

        target.PropPrivateGet = "test";
        target.GetProp().Should().Be("test");
    }

    [Fact]
    public async Task RoozieTestClassWithAttribute_VerifyInterface()
    {
        IRoozieTestClassWithAttribute target = new RoozieTestClassWithAttribute();

        target.NoParamMethodWithAttribute().Should().BeTrue();
        (await target.DoSomethingAsyncWithAttribute(42)).Should().Be(84);
        target.PropertyNormalWithAttribute.Should().Be("prop");
        target.PropertyGetWithAttribute.Should().Be(1234);

        target.PropPrivateGetWithAttribute = "test";
        target.GetPropWithAttribute().Should().Be("test");
    }
}
