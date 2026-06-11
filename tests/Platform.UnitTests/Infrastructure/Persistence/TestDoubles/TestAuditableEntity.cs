using Platform.Domain.Common;

namespace Platform.UnitTests.Infrastructure.Persistence.TestDoubles;

public sealed class TestAuditableEntity : AuditableEntity<Guid>, ISoftDeletable
{
    public TestAuditableEntity(Guid id, string name)
        : base(id)
    {
        Name = name;
    }

    private TestAuditableEntity()
    {
    }

    public string Name { get; set; } = string.Empty;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
