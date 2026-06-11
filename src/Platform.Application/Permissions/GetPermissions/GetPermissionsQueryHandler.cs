using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Domain.Results;

namespace Platform.Application.Permissions.GetPermissions;

public sealed class GetPermissionsQueryHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyList<PermissionDto>>>
{
    public async Task<Result<IReadOnlyList<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .OrderBy(permission => permission.Code)
            .Select(permission => new PermissionDto(permission.Id, permission.Code, permission.Name, permission.Description))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<PermissionDto>>(permissions);
    }
}
