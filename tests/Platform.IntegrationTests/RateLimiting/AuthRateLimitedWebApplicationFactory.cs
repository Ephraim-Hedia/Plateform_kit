using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Platform.IntegrationTests.RateLimiting;

public class AuthRateLimitedWebApplicationFactory : CustomWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RateLimiting:Register:PermitLimit"] = "1",
                ["RateLimiting:Login:PermitLimit"] = "1",
                ["RateLimiting:RefreshToken:PermitLimit"] = "1"
            });
        });
    }
}
