using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Platform.Infrastructure.Persistence;

namespace Platform.Infrastructure.Authorization;

public sealed class PermissionService(ApplicationDbContext dbContext, IMemoryCache cache) : IPermissionService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);

        foreach (var roleName in roleNames.Distinct(StringComparer.Ordinal))
        {
            var permissions = await cache.GetOrCreateAsync(PermissionCacheKeys.ForRole(roleName), async entry =>
            {
                entry.SetAbsoluteExpiration(CacheDuration);

                return await dbContext.Roles
                    .AsNoTracking()
                    .Where(role => role.Name == roleName)
                    .Join(dbContext.RolePermissions, role => role.Id, rolePermission => rolePermission.RoleId, (_, rolePermission) => rolePermission)
                    .Join(dbContext.Permissions, rolePermission => rolePermission.PermissionId, permission => permission.Id, (_, permission) => permission.Code)
                    .ToListAsync(cancellationToken);
            });

            if (permissions is not null)
            {
                result.UnionWith(permissions);
            }
        }

        return result;
    }
}
