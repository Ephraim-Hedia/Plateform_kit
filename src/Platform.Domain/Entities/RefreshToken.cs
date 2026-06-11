using System.Security.Cryptography;
using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public class RefreshToken : AuditableEntity<Guid>
{
    private RefreshToken(Guid id, Guid userId, string token, DateTimeOffset expiresAt)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    private RefreshToken()
    {
    }

    public Guid UserId { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    public bool IsRevoked => RevokedAt is not null;

    public bool IsActive => !IsRevoked && !IsExpired;

    public static RefreshToken Create(Guid userId, DateTimeOffset expiresAt) =>
        new(Guid.NewGuid(), userId, GenerateToken(), expiresAt);

    public void Revoke(string? replacedByToken = null)
    {
        RevokedAt = DateTimeOffset.UtcNow;
        ReplacedByToken = replacedByToken;
    }

    private static string GenerateToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
