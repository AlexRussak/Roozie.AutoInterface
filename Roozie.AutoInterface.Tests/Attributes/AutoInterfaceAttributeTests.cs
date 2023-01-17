using FluentAssertions;

namespace Roozie.AutoInterface.Tests.Attributes;

public class AutoInterfaceAttributeTests
{
    [Fact]
    public void VerifyProperties_AreTrue()
    {
        var target = new AutoInterfaceAttribute();

        target.IncludeMethods.Should().BeTrue();
        target.IncludeProperties.Should().BeTrue();
        target.ImplementOnPartial.Should().BeTrue();
    }
}
