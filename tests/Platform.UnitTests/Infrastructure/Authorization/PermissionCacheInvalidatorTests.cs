using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Platform.Domain.Entities;
using Platform.Infrastructure.Authorization;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Persistence;

namespace Platform.UnitTests.Infrastructure.Authorization;

public class PermissionCacheInvalidatorTests
{
    [Fact]
    public async Task InvalidateRole_Should_ForceReload_OfPermissionsForThatRole()
    {
        var dbContext = CreateDbContext();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var permissionService = new PermissionService(dbContext, cache);
        var invalidator = new PermissionCacheInvalidator(cache);

        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Manager", NormalizedName = "MANAGER" };
        var permission = Permission.Create("Reports.View", "View Reports");
        dbContext.Roles.Add(role);
        dbContext.Permissions.Add(permission);
        dbContext.RolePermissions.Add(RolePermission.Create(role.Id, permission.Id));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var initial = await permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);
        initial.Should().Contain("Reports.View");

        var newPermission = Permission.Create("Reports.Export", "Export Reports");
        dbContext.Permissions.Add(newPermission);
        dbContext.RolePermissions.Add(RolePermission.Create(role.Id, newPermission.Id));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var stale = await permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);
        stale.Should().NotContain("Reports.Export");

        invalidator.InvalidateRole("Manager");

        var refreshed = await permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);
        refreshed.Should().Contain("Reports.Export");
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
