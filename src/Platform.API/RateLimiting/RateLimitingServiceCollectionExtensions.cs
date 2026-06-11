using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Platform.API.RateLimiting;

public static class RateLimitingServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext => CreatePartition(httpContext, GetOptions(httpContext).Global));

            rateLimiterOptions.AddPolicy(RateLimitingPolicies.Login,
                httpContext => CreatePartition(httpContext, GetOptions(httpContext).Login));

            rateLimiterOptions.AddPolicy(RateLimitingPolicies.Register,
                httpContext => CreatePartition(httpContext, GetOptions(httpContext).Register));

            rateLimiterOptions.AddPolicy(RateLimitingPolicies.RefreshToken,
                httpContext => CreatePartition(httpContext, GetOptions(httpContext).RefreshToken));

            rateLimiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                }

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too Many Requests",
                        Detail = "Rate limit exceeded. Please try again later."
                    },
                    options: null,
                    contentType: "application/problem+json",
                    cancellationToken);
            };
        });

        return services;
    }

    private static RateLimitingOptions GetOptions(HttpContext httpContext) =>
        httpContext.RequestServices.GetRequiredService<IOptionsMonitor<RateLimitingOptions>>().CurrentValue;

    private static RateLimitPartition<string> CreatePartition(HttpContext httpContext, RateLimitPolicyOptions policyOptions)
    {
        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = policyOptions.PermitLimit,
            Window = TimeSpan.FromSeconds(policyOptions.WindowSeconds)
        });
    }
}
