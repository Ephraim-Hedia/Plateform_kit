using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Platform.Domain.Errors;
using Platform.Domain.Results;
using Platform.Infrastructure.Identity;
using Platform.UnitTests.Infrastructure.Identity.TestDoubles;

namespace Platform.UnitTests.Infrastructure.Identity;

public class IdentityServiceTests
{
    private readonly UserManager<ApplicationUser> _userManager = IdentityTestHelpers.CreateUserManager();
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityService _identityService;

    public IdentityServiceTests()
    {
        _signInManager = IdentityTestHelpers.CreateSignInManager(_userManager);
        _identityService = new IdentityService(_userManager, _signInManager);
    }

    [Fact]
    public async Task CreateUserAsync_Should_ReturnUserId_WhenCreationSucceeds()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Success));

        var result = await _identityService.CreateUserAsync("user@example.com", "Password123!", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateUserAsync_Should_ReturnEmailAlreadyExists_WhenEmailIsDuplicate()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email already taken."
            })));

        var result = await _identityService.CreateUserAsync("user@example.com", "Password123!", CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.EmailAlreadyExists);
    }

    [Fact]
    public async Task CreateUserAsync_Should_ReturnValidationError_ForOtherFailures()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password is too short."
            })));

        var result = await _identityService.CreateUserAsync("user@example.com", "short", CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("PasswordTooShort");
    }

    [Fact]
    public async Task ValidateCredentialsAsync_Should_ReturnAuthenticatedUser_WhenCredentialsAreValid()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com" };
        _userManager.FindByEmailAsync(user.Email).Returns(Task.FromResult<ApplicationUser?>(user));
        _signInManager.CheckPasswordSignInAsync(user, "Password123!", true)
            .Returns(Task.FromResult(SignInResult.Success));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(["User"]));

        var result = await _identityService.ValidateCredentialsAsync(user.Email, "Password123!", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
        result.Value.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task ValidateCredentialsAsync_Should_ReturnInvalidCredentials_WhenUserDoesNotExist()
    {
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns(Task.FromResult<ApplicationUser?>(null));

        var result = await _identityService.ValidateCredentialsAsync("missing@example.com", "Password123!", CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_Should_ReturnInvalidCredentials_WhenPasswordIsIncorrect()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com" };
        _userManager.FindByEmailAsync(user.Email).Returns(Task.FromResult<ApplicationUser?>(user));
        _signInManager.CheckPasswordSignInAsync(user, "wrong-password", true)
            .Returns(Task.FromResult(SignInResult.Failed));

        var result = await _identityService.ValidateCredentialsAsync(user.Email, "wrong-password", CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
    }

    [Fact]
    public async Task GetUserAsync_Should_ReturnAuthenticatedUser_WhenUserExists()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com" };
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(Task.FromResult<ApplicationUser?>(user));
        _userManager.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(["User"]));

        var result = await _identityService.GetUserAsync(user.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserAsync_Should_ReturnUserNotFound_WhenUserDoesNotExist()
    {
        _userManager.FindByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<ApplicationUser?>(null));

        var result = await _identityService.GetUserAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.UserNotFound);
    }
}
