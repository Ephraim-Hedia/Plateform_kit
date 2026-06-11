using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Application.Roles.GetRoles;
using Platform.Domain.Entities;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Roles.GetRoles;

public class GetRolesQueryHandlerTests
{
    private readonly TestApplicationDbContext _dbContext = CreateDbContext();
    private readonly IRoleService _roleService = Substitute.For<IRoleService>();
    private readonly GetRolesQueryHandler _handler;

    public GetRolesQueryHandlerTests()
    {
        _handler = new GetRolesQueryHandler(_roleService, _dbContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnRolesWithAssignedPermissions()
    {
        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        _roleService.GetRolesAsync(Arg.Any<CancellationToken>()).Returns(
        [
            new RoleSummary(adminRoleId, "Administrator"),
            new RoleSummary(userRoleId, "User")
        ]);

        var viewPermission = Permission.Create("Roles.View", "View Roles");
        var managePermission = Permission.Create("Roles.Manage", "Manage Roles");
        _dbContext.Permissions.AddRange(viewPermission, managePermission);
        _dbContext.RolePermissions.AddRange(
            RolePermission.Create(adminRoleId, viewPermission.Id),
            RolePermission.Create(adminRoleId, managePermission.Id));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new GetRolesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);

        var admin = result.Value.Single(r => r.Id == adminRoleId);
        admin.Permissions.Should().BeEquivalentTo(["Roles.Manage", "Roles.View"]);

        var user = result.Value.Single(r => r.Id == userRoleId);
        user.Permissions.Should().BeEmpty();
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
