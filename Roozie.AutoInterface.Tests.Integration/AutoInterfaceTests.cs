namespace Roozie.AutoInterface.Tests.Integration;

public class AutoInterfaceTests
{
    [Fact]
    public async Task RoozieTestClassPartial_VerifyInterface()
    {
        IRoozieTestClassPartial target = new RoozieTestClassPartial();

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

    [Fact]
    public void IndexerClass_VerifyInterface()
    {
        IIndexerClass target = new IndexerClass();

        target[1].Should().Be(0);
        target[2] = 1234;
        target[3].Should().Be(1234);

        target["Hello"].Should().Be("test");
        target["World"] = "foo";
        target["!"].Should().Be("foo");

        target["bar", 2].Should().Be(1234);
        target["baz", 3] = 5678;
        target["bin", 4].Should().Be(5678);
    }
}
