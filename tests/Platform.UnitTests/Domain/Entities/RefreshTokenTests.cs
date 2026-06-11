using FluentAssertions;
using Platform.Domain.Entities;

namespace Platform.UnitTests.Domain.Entities;

public class RefreshTokenTests
{
    [Fact]
    public void Create_Should_SetProperties()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        var refreshToken = RefreshToken.Create(userId, expiresAt);

        refreshToken.UserId.Should().Be(userId);
        refreshToken.ExpiresAt.Should().Be(expiresAt);
        refreshToken.Token.Should().NotBeNullOrWhiteSpace();
        refreshToken.RevokedAt.Should().BeNull();
        refreshToken.ReplacedByToken.Should().BeNull();
    }

    [Fact]
    public void Create_Should_GenerateUniqueTokens()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        var first = RefreshToken.Create(userId, expiresAt);
        var second = RefreshToken.Create(userId, expiresAt);

        first.Token.Should().NotBe(second.Token);
    }

    [Fact]
    public void IsExpired_Should_BeFalse_WhenExpiresAtIsInFuture()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_Should_BeTrue_WhenExpiresAtIsInPast()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(-1));

        refreshToken.IsExpired.Should().BeTrue();
    }

    [Fact]
    public void IsRevoked_Should_BeFalse_ByDefault()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void Revoke_Should_SetRevokedAt_And_ReplacedByToken()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.Revoke("new-token-value");

        refreshToken.IsRevoked.Should().BeTrue();
        refreshToken.RevokedAt.Should().NotBeNull();
        refreshToken.ReplacedByToken.Should().Be("new-token-value");
    }

    [Fact]
    public void IsActive_Should_BeTrue_WhenNotExpiredAndNotRevoked()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_Should_BeFalse_WhenRevoked()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));

        refreshToken.Revoke();

        refreshToken.IsActive.Should().BeFalse();
    }

    [Fact]
    public void IsActive_Should_BeFalse_WhenExpired()
    {
        var refreshToken = RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(-1));

        refreshToken.IsActive.Should().BeFalse();
    }
}
