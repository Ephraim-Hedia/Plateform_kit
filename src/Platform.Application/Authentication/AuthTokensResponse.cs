namespace Platform.Application.Authentication;

public sealed record AuthTokensResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
