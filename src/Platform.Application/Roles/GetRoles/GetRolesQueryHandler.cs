using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Domain.Results;

namespace Platform.Application.Roles.GetRoles;

public sealed class GetRolesQueryHandler(IRoleService roleService, IApplicationDbContext dbContext)
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRolesAsync(cancellationToken);

        var rolePermissions = await dbContext.RolePermissions
            .AsNoTracking()
            .Join(
                dbContext.Permissions,
                rolePermission => rolePermission.PermissionId,
                permission => permission.Id,
                (rolePermission, permission) => new { rolePermission.RoleId, permission.Code })
            .ToListAsync(cancellationToken);

        var permissionsByRole = rolePermissions
            .GroupBy(rolePermission => rolePermission.RoleId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<string>)group.Select(rolePermission => rolePermission.Code).OrderBy(code => code).ToList());

        var result = roles
            .Select(role => new RoleDto(
                role.Id,
                role.Name,
                permissionsByRole.TryGetValue(role.Id, out var codes) ? codes : []))
            .ToList();

        return Result.Success<IReadOnlyList<RoleDto>>(result);
    }
}
