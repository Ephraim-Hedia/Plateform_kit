using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;

namespace Platform.UnitTests.Application.Authentication.TestDoubles;

public sealed class TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Platform.Domain.Entities.RefreshToken> RefreshTokens => Set<Platform.Domain.Entities.RefreshToken>();
}
