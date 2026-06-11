using FluentAssertions;
using Platform.Application.Authentication.Register;

namespace Platform.UnitTests.Application.Authentication.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Succeed_WhenCommandIsValid()
    {
        var result = _validator.Validate(new RegisterCommand("user@example.com", "Password123!"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_Should_Fail_WhenEmailIsInvalid(string email)
    {
        var result = _validator.Validate(new RegisterCommand(email, "Password123!"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public void Validate_Should_Fail_WhenPasswordIsInvalid(string password)
    {
        var result = _validator.Validate(new RegisterCommand("user@example.com", password));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterCommand.Password));
    }
}
