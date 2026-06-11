using Platform.Domain.Common;

namespace Platform.UnitTests.Domain.Common.TestDoubles;

internal sealed class TestDomainEvent : IDomainEvent
{
}

internal sealed class TestAggregateRoot : AggregateRoot<Guid>
{
    public TestAggregateRoot(Guid id)
        : base(id)
    {
    }

    public void RaiseTestEvent(IDomainEvent domainEvent) => RaiseDomainEvent(domainEvent);
}
