using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Domain.Entities;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Application.Roles.AssignPermissions;

public sealed class AssignPermissionsCommandHandler(
    IRoleService roleService,
    IApplicationDbContext dbContext,
    IPermissionCacheInvalidator cacheInvalidator)
    : IRequestHandler<AssignPermissionsCommand, Result>
{
    public async Task<Result> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
    {
        var roleResult = await roleService.GetRoleAsync(request.RoleId, cancellationToken);

        if (roleResult.IsFailure)
        {
            return Result.Failure(roleResult.Error);
        }

        var distinctCodes = request.PermissionCodes.Distinct().ToArray();

        var permissions = await dbContext.Permissions
            .Where(permission => distinctCodes.Contains(permission.Code))
            .ToListAsync(cancellationToken);

        if (permissions.Count != distinctCodes.Length)
        {
            return Result.Failure(AuthorizationErrors.PermissionNotFound);
        }

        var existingRolePermissions = await dbContext.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == request.RoleId)
            .ToListAsync(cancellationToken);

        dbContext.RolePermissions.RemoveRange(existingRolePermissions);

        foreach (var permission in permissions)
        {
            dbContext.RolePermissions.Add(RolePermission.Create(request.RoleId, permission.Id));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        cacheInvalidator.InvalidateRole(roleResult.Value.Name);

        return Result.Success();
    }
}
