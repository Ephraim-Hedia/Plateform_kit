using FluentAssertions;
using Platform.UnitTests.Domain.Common.TestDoubles;

namespace Platform.UnitTests.Domain.Common;

public class AggregateRootTests
{
    [Fact]
    public void DomainEvents_Should_BeEmpty_When_AggregateIsCreated()
    {
        var aggregate = new TestAggregateRoot(Guid.NewGuid());

        aggregate.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void RaiseDomainEvent_Should_AddEventToDomainEvents()
    {
        var aggregate = new TestAggregateRoot(Guid.NewGuid());
        var domainEvent = new TestDomainEvent();

        aggregate.RaiseTestEvent(domainEvent);

        aggregate.DomainEvents.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void ClearDomainEvents_Should_RemoveAllRaisedEvents()
    {
        var aggregate = new TestAggregateRoot(Guid.NewGuid());
        aggregate.RaiseTestEvent(new TestDomainEvent());
        aggregate.RaiseTestEvent(new TestDomainEvent());

        aggregate.ClearDomainEvents();

        aggregate.DomainEvents.Should().BeEmpty();
    }
}
