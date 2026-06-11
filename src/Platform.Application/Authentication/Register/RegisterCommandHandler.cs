using MediatR;
using Platform.Application.Abstractions;
using Platform.Domain.Results;

namespace Platform.Application.Authentication.Register;

public sealed class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        identityService.CreateUserAsync(request.Email, request.Password, cancellationToken);
}
