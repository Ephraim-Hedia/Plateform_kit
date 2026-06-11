using Microsoft.EntityFrameworkCore;
using Platform.Domain.Entities;

namespace Platform.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<RolePermission> RolePermissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
