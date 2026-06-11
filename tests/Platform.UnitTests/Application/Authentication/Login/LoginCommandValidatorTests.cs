using FluentAssertions;
using Platform.Application.Authentication.Login;

namespace Platform.UnitTests.Application.Authentication.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Succeed_WhenCommandIsValid()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", "Password123!"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_Should_Fail_WhenEmailIsInvalid(string email)
    {
        var result = _validator.Validate(new LoginCommand(email, "Password123!"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Email));
    }

    [Fact]
    public void Validate_Should_Fail_WhenPasswordIsEmpty()
    {
        var result = _validator.Validate(new LoginCommand("user@example.com", string.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LoginCommand.Password));
    }
}
