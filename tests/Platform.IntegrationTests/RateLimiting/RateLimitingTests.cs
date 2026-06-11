using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Platform.IntegrationTests.RateLimiting;

public class RateLimitingTests : IClassFixture<AuthRateLimitedWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitingTests(AuthRateLimitedWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Should_ReturnTooManyRequests_WhenLimitExceeded()
    {
        var first = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new { Email = $"{Guid.NewGuid()}@example.com", Password = "Password123!" });
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await _client.PostAsJsonAsync(
            "/api/v1/auth/register",
            new { Email = $"{Guid.NewGuid()}@example.com", Password = "Password123!" });

        second.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        second.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        second.Headers.RetryAfter.Should().NotBeNull();
    }
}
