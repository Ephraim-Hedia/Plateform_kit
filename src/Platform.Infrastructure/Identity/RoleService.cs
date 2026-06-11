using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Infrastructure.Identity;

public sealed class RoleService(RoleManager<ApplicationRole> roleManager) : IRoleService
{
    public async Task<IReadOnlyList<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken)
    {
        return await roleManager.Roles
            .AsNoTracking()
            .Select(role => new RoleSummary(role.Id, role.Name!))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<RoleSummary>> GetRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(roleId.ToString());

        if (role is null)
        {
            return Result.Failure<RoleSummary>(AuthorizationErrors.RoleNotFound);
        }

        return Result.Success(new RoleSummary(role.Id, role.Name!));
    }
}
