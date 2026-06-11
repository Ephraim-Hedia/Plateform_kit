using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Platform.Application.Abstractions;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    IIdentityService identityService,
    IJwtTokenGenerator tokenGenerator,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokensResponse>>
{
    public async Task<Result<AuthTokensResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            return Result.Failure<AuthTokensResponse>(AuthenticationErrors.InvalidRefreshToken);
        }

        var userResult = await identityService.GetUserAsync(existingToken.UserId, cancellationToken);

        if (userResult.IsFailure)
        {
            return Result.Failure<AuthTokensResponse>(userResult.Error);
        }

        var user = userResult.Value;

        var newRefreshToken = Domain.Entities.RefreshToken.Create(
            user.Id,
            DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays));

        existingToken.Revoke(newRefreshToken.Token);
        dbContext.RefreshTokens.Add(newRefreshToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles);

        return Result.Success(new AuthTokensResponse(accessToken, newRefreshToken.Token, newRefreshToken.ExpiresAt));
    }
}
