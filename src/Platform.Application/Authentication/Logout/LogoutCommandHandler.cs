using MediatR;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Abstractions;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Logout;

public sealed class LogoutCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (token is null || !token.IsActive)
        {
            return Result.Failure(AuthenticationErrors.InvalidRefreshToken);
        }

        token.Revoke();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
