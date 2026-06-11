using Platform.Domain.Results;

namespace Platform.Application.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleSummary>> GetRolesAsync(CancellationToken cancellationToken);

    Task<Result<RoleSummary>> GetRoleAsync(Guid roleId, CancellationToken cancellationToken);
}

public sealed record RoleSummary(Guid Id, string Name);
