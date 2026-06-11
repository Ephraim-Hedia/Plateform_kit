using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Platform.Application.Authentication;

namespace Platform.IntegrationTests.Authentication;

public class AuthenticationFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _client;

    public AuthenticationFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullAuthenticationFlow_Should_Succeed()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "Password123!";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new { Email = email, Password = password });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthTokensResponse>(JsonOptions);
        tokens.Should().NotBeNull();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);
        var meResponse = await _client.GetAsync("/api/v1/auth/me");
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        _client.DefaultRequestHeaders.Authorization = null;
        var refreshResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new { RefreshToken = tokens.RefreshToken });
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var newTokens = await refreshResponse.Content.ReadFromJsonAsync<AuthTokensResponse>(JsonOptions);
        newTokens.Should().NotBeNull();
        newTokens!.RefreshToken.Should().NotBe(tokens.RefreshToken);

        var logoutResponse = await _client.PostAsJsonAsync("/api/v1/auth/logout", new { RefreshToken = newTokens.RefreshToken });
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var reuseOldTokenResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new { RefreshToken = tokens.RefreshToken });
        reuseOldTokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var reuseRevokedTokenResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new { RefreshToken = newTokens.RefreshToken });
        reuseRevokedTokenResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_Should_ReturnUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/api/v1/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_Should_ReturnConflict_WhenEmailAlreadyExists()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "Password123!";

        await _client.PostAsJsonAsync("/api/v1/auth/register", new { Email = email, Password = password });
        var secondResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new { Email = email, Password = password });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_WithInvalidCredentials()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = "nonexistent@example.com", Password = "WrongPassword123!" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnUnauthorized_WhenTokenIsInvalid()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh-token", new { RefreshToken = "not-a-real-token" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
