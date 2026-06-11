namespace Platform.API.RateLimiting;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public RateLimitPolicyOptions Global { get; set; } = new() { PermitLimit = 100, WindowSeconds = 60 };

    public RateLimitPolicyOptions Login { get; set; } = new() { PermitLimit = 5, WindowSeconds = 60 };

    public RateLimitPolicyOptions Register { get; set; } = new() { PermitLimit = 3, WindowSeconds = 60 };

    public RateLimitPolicyOptions RefreshToken { get; set; } = new() { PermitLimit = 10, WindowSeconds = 60 };
}

public sealed class RateLimitPolicyOptions
{
    public int PermitLimit { get; set; }

    public int WindowSeconds { get; set; }
}
