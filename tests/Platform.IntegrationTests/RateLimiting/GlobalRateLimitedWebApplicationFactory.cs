using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Platform.IntegrationTests.RateLimiting;

public class GlobalRateLimitedWebApplicationFactory : CustomWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RateLimiting:Global:PermitLimit"] = "1"
            });
        });
    }
}
