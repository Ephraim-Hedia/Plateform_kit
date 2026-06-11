using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Infrastructure.Persistence.Interceptors;
using Platform.UnitTests.Infrastructure.Persistence.TestDoubles;

namespace Platform.UnitTests.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptorTests
{
    private static TestDbContext CreateContext(ICurrentUser currentUser)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public void SaveChanges_Should_SetCreatedAuditFields_When_EntityIsAdded()
    {
        var userId = Guid.NewGuid();
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(userId);

        using var context = CreateContext(currentUser);
        var entity = new TestAuditableEntity(Guid.NewGuid(), "test");

        context.TestEntities.Add(entity);
        context.SaveChanges();

        entity.CreatedBy.Should().Be(userId);
        entity.CreatedAt.Should().NotBe(default);
        entity.UpdatedAt.Should().BeNull();
        entity.UpdatedBy.Should().BeNull();
    }

    [Fact]
    public void SaveChanges_Should_SetUpdatedAuditFields_When_EntityIsModified()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns((Guid?)null);

        using var context = CreateContext(currentUser);
        var entity = new TestAuditableEntity(Guid.NewGuid(), "test");
        context.TestEntities.Add(entity);
        context.SaveChanges();

        var updaterId = Guid.NewGuid();
        currentUser.UserId.Returns(updaterId);

        entity.Name = "updated";
        context.SaveChanges();

        entity.UpdatedBy.Should().Be(updaterId);
        entity.UpdatedAt.Should().NotBeNull();
    }
}
