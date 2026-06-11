using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Roles.AssignPermissions;

public sealed record AssignPermissionsCommand(Guid RoleId, IReadOnlyList<string> PermissionCodes) : IRequest<Result>;
