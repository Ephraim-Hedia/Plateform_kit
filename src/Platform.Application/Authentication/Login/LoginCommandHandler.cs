using MediatR;
using Microsoft.Extensions.Options;
using Platform.Application.Abstractions;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IJwtTokenGenerator tokenGenerator,
    IApplicationDbContext dbContext,
    IOptions<JwtOptions> jwtOptions)
    : IRequestHandler<LoginCommand, Result<AuthTokensResponse>>
{
    public async Task<Result<AuthTokensResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userResult = await identityService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

        if (userResult.IsFailure)
        {
            return Result.Failure<AuthTokensResponse>(userResult.Error);
        }

        var user = userResult.Value;
        var accessToken = tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles);

        var refreshToken = Domain.Entities.RefreshToken.Create(
            user.Id,
            DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationDays));

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokensResponse(accessToken, refreshToken.Token, refreshToken.ExpiresAt));
    }
}
