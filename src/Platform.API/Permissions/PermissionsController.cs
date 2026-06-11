using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Platform.API.Common;
using Platform.Application.Permissions.GetPermissions;
using Platform.Infrastructure.Authorization;
using PermissionConstants = Platform.Domain.Constants.Permissions;

namespace Platform.API.Permissions;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/permissions")]
public sealed class PermissionsController(ISender sender) : ApiControllerBase
{
    [HasPermission(PermissionConstants.PermissionCatalog.View)]
    [HttpGet]
    public async Task<IActionResult> GetPermissions(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPermissionsQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }
}
