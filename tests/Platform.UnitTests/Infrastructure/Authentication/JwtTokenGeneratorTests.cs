using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Platform.Application.Authentication;
using Platform.Infrastructure.Authentication;

namespace Platform.UnitTests.Infrastructure.Authentication;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _generator = new(Options.Create(new JwtOptions
    {
        Secret = "unit-test-secret-unit-test-secret",
        Issuer = "unit-test-issuer",
        Audience = "unit-test-audience",
        AccessTokenExpirationMinutes = 15,
        RefreshTokenExpirationDays = 7
    }));

    [Fact]
    public void GenerateAccessToken_Should_IncludeUserClaims()
    {
        var userId = Guid.NewGuid();
        const string email = "user@example.com";
        string[] roles = ["Admin", "User"];

        var token = _generator.GenerateAccessToken(userId, email, roles);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
        jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).Should().BeEquivalentTo(roles);
    }

    [Fact]
    public void GenerateAccessToken_Should_SetIssuerAndAudience()
    {
        var token = _generator.GenerateAccessToken(Guid.NewGuid(), "user@example.com", []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.Issuer.Should().Be("unit-test-issuer");
        jwt.Audiences.Should().Contain("unit-test-audience");
    }

    [Fact]
    public void GenerateAccessToken_Should_SetExpiration_BasedOnOptions()
    {
        var expectedExpiration = DateTime.UtcNow.AddMinutes(15);

        var token = _generator.GenerateAccessToken(Guid.NewGuid(), "user@example.com", []);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        jwt.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromSeconds(5));
    }
}
