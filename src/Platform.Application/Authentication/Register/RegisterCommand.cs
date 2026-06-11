using MediatR;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Register;

public sealed record RegisterCommand(string Email, string Password) : IRequest<Result<Guid>>;
