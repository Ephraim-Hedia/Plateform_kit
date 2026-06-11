using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Permissions.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionDto>>>;
