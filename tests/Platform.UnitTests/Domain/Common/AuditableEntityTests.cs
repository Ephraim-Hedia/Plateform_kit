using FluentAssertions;
using Platform.UnitTests.Domain.Common.TestDoubles;

namespace Platform.UnitTests.Domain.Common;

public class AuditableEntityTests
{
    [Fact]
    public void AuditProperties_Should_BeSettable()
    {
        var entity = new TestAuditableEntity(Guid.NewGuid());
        var createdAt = DateTimeOffset.UtcNow;
        var updatedAt = createdAt.AddHours(1);
        var createdBy = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();

        entity.CreatedAt = createdAt;
        entity.CreatedBy = createdBy;
        entity.UpdatedAt = updatedAt;
        entity.UpdatedBy = updatedBy;

        entity.CreatedAt.Should().Be(createdAt);
        entity.CreatedBy.Should().Be(createdBy);
        entity.UpdatedAt.Should().Be(updatedAt);
        entity.UpdatedBy.Should().Be(updatedBy);
    }

    [Fact]
    public void UpdateAuditProperties_Should_BeNull_ByDefault()
    {
        var entity = new TestAuditableEntity(Guid.NewGuid());

        entity.UpdatedAt.Should().BeNull();
        entity.UpdatedBy.Should().BeNull();
        entity.CreatedBy.Should().BeNull();
    }
}
