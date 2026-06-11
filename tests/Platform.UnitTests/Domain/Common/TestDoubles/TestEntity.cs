using Platform.Domain.Common;

namespace Platform.UnitTests.Domain.Common.TestDoubles;

internal sealed class TestEntity : Entity<Guid>
{
    public TestEntity(Guid id)
        : base(id)
    {
    }
}

internal sealed class OtherTestEntity : Entity<Guid>
{
    public OtherTestEntity(Guid id)
        : base(id)
    {
    }
}
