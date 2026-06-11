using Platform.Domain.Common;

namespace Platform.UnitTests.Domain.Common.TestDoubles;

internal sealed class TestAuditableEntity : AuditableEntity<Guid>
{
    public TestAuditableEntity(Guid id)
        : base(id)
    {
    }
}
