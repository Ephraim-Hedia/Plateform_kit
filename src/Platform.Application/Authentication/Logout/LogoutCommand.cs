using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
