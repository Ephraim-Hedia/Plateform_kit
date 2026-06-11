namespace Platform.Domain.Constants;

public static class Permissions
{
    public static class Roles
    {
        public const string View = "Roles.View";

        public const string Manage = "Roles.Manage";
    }

    public static class PermissionCatalog
    {
        public const string View = "Permissions.View";
    }

    public static IReadOnlyList<string> All { get; } =
    [
        Roles.View,
        Roles.Manage,
        PermissionCatalog.View
    ];
}
