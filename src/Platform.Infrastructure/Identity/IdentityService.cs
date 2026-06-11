using Microsoft.AspNetCore.Identity;
using Platform.Application.Abstractions;
using Platform.Application.Authentication;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code is "DuplicateUserName" or "DuplicateEmail"))
            {
                return Result.Failure<Guid>(AuthenticationErrors.EmailAlreadyExists);
            }

            var firstError = result.Errors.First();
            return Result.Failure<Guid>(Error.Validation(firstError.Code, firstError.Description));
        }

        return Result.Success(user.Id);
    }

    public async Task<Result<AuthenticatedUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<AuthenticatedUser>(AuthenticationErrors.InvalidCredentials);
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return Result.Failure<AuthenticatedUser>(AuthenticationErrors.InvalidCredentials);
        }

        return await ToAuthenticatedUserAsync(user);
    }

    public async Task<Result<AuthenticatedUser>> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure<AuthenticatedUser>(AuthenticationErrors.UserNotFound);
        }

        return await ToAuthenticatedUserAsync(user);
    }

    private async Task<Result<AuthenticatedUser>> ToAuthenticatedUserAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return Result.Success(new AuthenticatedUser(user.Id, user.Email!, roles.ToArray()));
    }
}
