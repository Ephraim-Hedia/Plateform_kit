using FluentAssertions;
using Platform.UnitTests.Domain.Common.TestDoubles;

namespace Platform.UnitTests.Domain.Common;

public class EntityTests
{
    [Fact]
    public void Equals_Should_ReturnTrue_When_SameTypeAndSameId()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new TestEntity(id);

        first.Equals(second).Should().BeTrue();
        (first == second).Should().BeTrue();
        (first != second).Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_DifferentIds()
    {
        var first = new TestEntity(Guid.NewGuid());
        var second = new TestEntity(Guid.NewGuid());

        first.Equals(second).Should().BeFalse();
        (first == second).Should().BeFalse();
        (first != second).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_DifferentTypes_WithSameId()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new OtherTestEntity(id);

        first.Equals(second).Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToNull()
    {
        var entity = new TestEntity(Guid.NewGuid());

        entity.Equals(null).Should().BeFalse();
        (entity == null).Should().BeFalse();
        (null == entity).Should().BeFalse();
        (entity != null).Should().BeTrue();
    }

    [Fact]
    public void EqualityOperator_Should_ReturnTrue_When_BothNull()
    {
        TestEntity? first = null;
        TestEntity? second = null;

        (first == second).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_Should_BeEqual_When_SameId()
    {
        var id = Guid.NewGuid();
        var first = new TestEntity(id);
        var second = new TestEntity(id);

        first.GetHashCode().Should().Be(second.GetHashCode());
    }
}
