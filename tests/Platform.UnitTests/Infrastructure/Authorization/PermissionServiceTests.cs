using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Platform.Domain.Entities;
using Platform.Infrastructure.Authorization;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Persistence;

namespace Platform.UnitTests.Infrastructure.Authorization;

public class PermissionServiceTests
{
    private readonly ApplicationDbContext _dbContext = CreateDbContext();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly PermissionService _permissionService;

    public PermissionServiceTests()
    {
        _permissionService = new PermissionService(_dbContext, _cache);
    }

    [Fact]
    public async Task GetPermissionsAsync_Should_ReturnPermissionsAssignedToRole()
    {
        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Manager", NormalizedName = "MANAGER" };
        var permission = Permission.Create("Reports.View", "View Reports");
        _dbContext.Roles.Add(role);
        _dbContext.Permissions.Add(permission);
        _dbContext.RolePermissions.Add(RolePermission.Create(role.Id, permission.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);

        result.Should().Contain("Reports.View");
    }

    [Fact]
    public async Task GetPermissionsAsync_Should_ReturnEmptySet_WhenRoleHasNoPermissions()
    {
        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Guest", NormalizedName = "GUEST" };
        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _permissionService.GetPermissionsAsync(["Guest"], CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPermissionsAsync_Should_CombinePermissions_FromMultipleRoles()
    {
        var roleA = new ApplicationRole { Id = Guid.NewGuid(), Name = "RoleA", NormalizedName = "ROLEA" };
        var roleB = new ApplicationRole { Id = Guid.NewGuid(), Name = "RoleB", NormalizedName = "ROLEB" };
        var permissionA = Permission.Create("A.View", "View A");
        var permissionB = Permission.Create("B.View", "View B");
        _dbContext.Roles.AddRange(roleA, roleB);
        _dbContext.Permissions.AddRange(permissionA, permissionB);
        _dbContext.RolePermissions.AddRange(
            RolePermission.Create(roleA.Id, permissionA.Id),
            RolePermission.Create(roleB.Id, permissionB.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _permissionService.GetPermissionsAsync(["RoleA", "RoleB"], CancellationToken.None);

        result.Should().BeEquivalentTo(["A.View", "B.View"]);
    }

    [Fact]
    public async Task GetPermissionsAsync_Should_CachePermissions_AcrossCalls()
    {
        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Manager", NormalizedName = "MANAGER" };
        var permission = Permission.Create("Reports.View", "View Reports");
        _dbContext.Roles.Add(role);
        _dbContext.Permissions.Add(permission);
        _dbContext.RolePermissions.Add(RolePermission.Create(role.Id, permission.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var first = await _permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);

        _dbContext.RolePermissions.RemoveRange(_dbContext.RolePermissions.Where(rp => rp.RoleId == role.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var second = await _permissionService.GetPermissionsAsync(["Manager"], CancellationToken.None);

        first.Should().Contain("Reports.View");
        second.Should().Contain("Reports.View");
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
