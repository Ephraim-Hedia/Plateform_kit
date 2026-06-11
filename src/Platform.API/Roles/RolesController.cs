using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Common;
using Platform.Application.Roles.AssignPermissions;
using Platform.Application.Roles.GetRoles;
using Platform.Infrastructure.Authorization;
using PermissionConstants = Platform.Domain.Constants.Permissions;

namespace Platform.API.Roles;

[Route("api/roles")]
public sealed class RolesController(ISender sender) : ApiControllerBase
{
    [HasPermission(PermissionConstants.Roles.View)]
    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HasPermission(PermissionConstants.Roles.Manage)]
    [HttpPut("{roleId:guid}/permissions")]
    public async Task<IActionResult> AssignPermissions(Guid roleId, AssignPermissionsRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignPermissionsCommand(roleId, request.PermissionCodes);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }
}

public sealed record AssignPermissionsRequest(IReadOnlyList<string> PermissionCodes);
