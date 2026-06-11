using Microsoft.EntityFrameworkCore;
using Platform.Infrastructure.Persistence.Extensions;

namespace Platform.UnitTests.Infrastructure.Persistence.TestDoubles;

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestAuditableEntity> TestEntities => Set<TestAuditableEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestAuditableEntity>();

        modelBuilder.ApplySoftDeleteQueryFilter();

        base.OnModelCreating(modelBuilder);
    }
}
