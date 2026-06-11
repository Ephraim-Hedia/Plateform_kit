using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Platform.Infrastructure.Persistence.Interceptors;
using Platform.UnitTests.Infrastructure.Persistence.TestDoubles;

namespace Platform.UnitTests.Infrastructure.Persistence.Interceptors;

public class SoftDeleteInterceptorTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public void SaveChanges_Should_ConvertDeleteToSoftDelete()
    {
        using var context = CreateContext();
        var entity = new TestAuditableEntity(Guid.NewGuid(), "test");
        context.TestEntities.Add(entity);
        context.SaveChanges();

        context.TestEntities.Remove(entity);
        context.SaveChanges();

        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Query_Should_ExcludeSoftDeletedEntities_ByDefault()
    {
        using var context = CreateContext();
        var entity = new TestAuditableEntity(Guid.NewGuid(), "test");
        context.TestEntities.Add(entity);
        context.SaveChanges();

        context.TestEntities.Remove(entity);
        context.SaveChanges();

        context.TestEntities.ToList().Should().BeEmpty();
        context.TestEntities.IgnoreQueryFilters().ToList().Should().ContainSingle();
    }
}
