using FluentAssertions;
using Platform.UnitTests.Domain.Common.TestDoubles;

namespace Platform.UnitTests.Domain.Common;

public class SoftDeletableTests
{
    [Fact]
    public void Entity_Should_NotBeDeleted_ByDefault()
    {
        var entity = new TestSoftDeletableEntity(Guid.NewGuid());

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void Entity_Should_BeMarkedAsDeleted_When_SoftDeleted()
    {
        var entity = new TestSoftDeletableEntity(Guid.NewGuid());
        var deletedAt = DateTimeOffset.UtcNow;

        entity.IsDeleted = true;
        entity.DeletedAt = deletedAt;

        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().Be(deletedAt);
    }
}
