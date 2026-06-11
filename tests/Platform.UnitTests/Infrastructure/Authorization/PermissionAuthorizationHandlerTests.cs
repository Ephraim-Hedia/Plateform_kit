using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using Platform.Infrastructure.Authorization;

namespace Platform.UnitTests.Infrastructure.Authorization;

public class PermissionAuthorizationHandlerTests
{
    private readonly IPermissionService _permissionService = Substitute.For<IPermissionService>();
    private readonly PermissionAuthorizationHandler _handler;

    public PermissionAuthorizationHandlerTests()
    {
        _handler = new PermissionAuthorizationHandler(_permissionService);
    }

    [Fact]
    public async Task HandleAsync_Should_Succeed_WhenUserHasRequiredPermission()
    {
        var requirement = new PermissionRequirement("Roles.View");
        var user = CreateUser("Administrator");

        _permissionService.GetPermissionsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<string> { "Roles.View" });

        var context = new AuthorizationHandlerContext([requirement], user, null);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_Should_NotSucceed_WhenUserLacksRequiredPermission()
    {
        var requirement = new PermissionRequirement("Roles.Manage");
        var user = CreateUser("Viewer");

        _permissionService.GetPermissionsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new HashSet<string> { "Roles.View" });

        var context = new AuthorizationHandlerContext([requirement], user, null);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeFalse();
    }

    private static ClaimsPrincipal CreateUser(params string[] roles)
    {
        var identity = new ClaimsIdentity(roles.Select(role => new Claim(ClaimTypes.Role, role)), "Test");

        return new ClaimsPrincipal(identity);
    }
}
