using Platform.Domain.Common;

namespace Platform.UnitTests.Domain.Common.TestDoubles;

internal sealed class TestSoftDeletableEntity : Entity<Guid>, ISoftDeletable
{
    public TestSoftDeletableEntity(Guid id)
        : base(id)
    {
    }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
