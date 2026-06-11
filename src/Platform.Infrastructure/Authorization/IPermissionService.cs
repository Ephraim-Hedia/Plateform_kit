namespace Platform.Infrastructure.Authorization;

public interface IPermissionService
{
    Task<IReadOnlySet<string>> GetPermissionsAsync(IEnumerable<string> roleNames, CancellationToken cancellationToken);
}
