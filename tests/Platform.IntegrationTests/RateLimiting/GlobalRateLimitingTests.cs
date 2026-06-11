using System.Net;
using FluentAssertions;

namespace Platform.IntegrationTests.RateLimiting;

public class GlobalRateLimitingTests : IClassFixture<GlobalRateLimitedWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GlobalRateLimitingTests(GlobalRateLimitedWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GlobalPolicy_Should_ReturnTooManyRequests_WhenLimitExceeded()
    {
        var first = await _client.GetAsync("/health/live");
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await _client.GetAsync("/health/live");

        second.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
