using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Platform.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roles = context.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value);

        var permissions = await permissionService.GetPermissionsAsync(roles, CancellationToken.None);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
