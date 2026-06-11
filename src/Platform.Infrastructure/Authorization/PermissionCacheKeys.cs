namespace Platform.Infrastructure.Authorization;

internal static class PermissionCacheKeys
{
    private const string Prefix = "permissions:role:";

    public static string ForRole(string roleName) => $"{Prefix}{roleName}";
}
