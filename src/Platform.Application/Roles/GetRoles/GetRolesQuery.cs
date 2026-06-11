using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Roles.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleDto>>>;
