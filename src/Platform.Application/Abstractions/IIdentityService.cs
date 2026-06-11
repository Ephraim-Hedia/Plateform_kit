using Platform.Application.Authentication;
using Platform.Domain.Results;

namespace Platform.Application.Abstractions;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> GetUserAsync(Guid userId, CancellationToken cancellationToken);
}
