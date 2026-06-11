using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Application.Roles.AssignPermissions;
using Platform.Domain.Entities;
using Platform.Domain.Errors;
using Platform.Domain.Results;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Roles.AssignPermissions;

public class AssignPermissionsCommandHandlerTests
{
    private readonly TestApplicationDbContext _dbContext = CreateDbContext();
    private readonly IRoleService _roleService = Substitute.For<IRoleService>();
    private readonly IPermissionCacheInvalidator _cacheInvalidator = Substitute.For<IPermissionCacheInvalidator>();
    private readonly AssignPermissionsCommandHandler _handler;

    public AssignPermissionsCommandHandlerTests()
    {
        _handler = new AssignPermissionsCommandHandler(_roleService, _dbContext, _cacheInvalidator);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenRoleNotFound()
    {
        var roleId = Guid.NewGuid();
        _roleService.GetRoleAsync(roleId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<RoleSummary>(AuthorizationErrors.RoleNotFound));

        var result = await _handler.Handle(new AssignPermissionsCommand(roleId, ["Roles.View"]), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthorizationErrors.RoleNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPermissionCodeDoesNotExist()
    {
        var roleId = Guid.NewGuid();
        _roleService.GetRoleAsync(roleId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(new RoleSummary(roleId, "Administrator")));

        var result = await _handler.Handle(new AssignPermissionsCommand(roleId, ["Unknown.Permission"]), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthorizationErrors.PermissionNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReplaceRolePermissions_AndInvalidateCache()
    {
        var roleId = Guid.NewGuid();
        _roleService.GetRoleAsync(roleId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(new RoleSummary(roleId, "Administrator")));

        var viewPermission = Permission.Create("Roles.View", "View Roles");
        var managePermission = Permission.Create("Roles.Manage", "Manage Roles");
        _dbContext.Permissions.AddRange(viewPermission, managePermission);
        _dbContext.RolePermissions.Add(RolePermission.Create(roleId, viewPermission.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new AssignPermissionsCommand(roleId, ["Roles.Manage"]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var rolePermissions = await _dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == roleId)
            .ToListAsync(CancellationToken.None);

        rolePermissions.Should().ContainSingle(rolePermission => rolePermission.PermissionId == managePermission.Id);

        _cacheInvalidator.Received(1).InvalidateRole("Administrator");
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
