using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthTokensResponse>>;
