using FluentAssertions;
using Platform.Application.Authentication.RefreshToken;

namespace Platform.UnitTests.Application.Authentication.RefreshToken;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Succeed_WhenRefreshTokenIsProvided()
    {
        var result = _validator.Validate(new RefreshTokenCommand("some-refresh-token"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenRefreshTokenIsEmpty()
    {
        var result = _validator.Validate(new RefreshTokenCommand(string.Empty));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RefreshTokenCommand.RefreshToken));
    }
}
