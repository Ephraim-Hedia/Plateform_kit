using FluentAssertions;
using Platform.Application.Authentication.Logout;

namespace Platform.UnitTests.Application.Authentication.Logout;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Succeed_WhenRefreshTokenIsProvided()
    {
        var result = _validator.Validate(new LogoutCommand("some-refresh-token"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenRefreshTokenIsEmpty()
    {
        var result = _validator.Validate(new LogoutCommand(string.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LogoutCommand.RefreshToken));
    }
}
