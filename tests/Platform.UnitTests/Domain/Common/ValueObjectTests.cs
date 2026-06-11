using FluentAssertions;
using Platform.UnitTests.Domain.Common.TestDoubles;

namespace Platform.UnitTests.Domain.Common;

public class ValueObjectTests
{
    [Fact]
    public void Equals_Should_ReturnTrue_When_AllComponentsAreEqual()
    {
        var first = new TestValueObject("Line1", "Line2");
        var second = new TestValueObject("Line1", "Line2");

        first.Equals(second).Should().BeTrue();
        (first == second).Should().BeTrue();
        (first != second).Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_AnyComponentDiffers()
    {
        var first = new TestValueObject("Line1", "Line2");
        var second = new TestValueObject("Line1", "Different");

        first.Equals(second).Should().BeFalse();
        (first == second).Should().BeFalse();
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_DifferentTypes_WithSameComponents()
    {
        var first = new TestValueObject("Line1", "Line2");
        var second = new OtherTestValueObject("Line1", "Line2");

        first.Equals(second).Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToNull()
    {
        var value = new TestValueObject("Line1", "Line2");

        value.Equals(null).Should().BeFalse();
        (value == null).Should().BeFalse();
        (value != null).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_Should_BeEqual_When_AllComponentsAreEqual()
    {
        var first = new TestValueObject("Line1", "Line2");
        var second = new TestValueObject("Line1", "Line2");

        first.GetHashCode().Should().Be(second.GetHashCode());
    }
}
