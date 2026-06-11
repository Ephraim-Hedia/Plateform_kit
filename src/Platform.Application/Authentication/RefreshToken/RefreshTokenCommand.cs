using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokensResponse>>;
