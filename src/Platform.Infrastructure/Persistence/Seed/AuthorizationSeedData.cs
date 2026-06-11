using Platform.Domain.Constants;
using Platform.Infrastructure.Identity;

namespace Platform.Infrastructure.Persistence.Seed;

internal static class AuthorizationSeedData
{
    public static readonly Guid AdministratorRoleId = Guid.Parse("9f4d8b1e-0000-0000-0000-000000000001");

    private static readonly DateTimeOffset SeededAt = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly PermissionSeedEntry[] Entries =
    [
        new(
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000101"),
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000201"),
            Permissions.Roles.View,
            "View Roles",
            "View roles and the permissions assigned to them."),

        new(
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000102"),
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000202"),
            Permissions.Roles.Manage,
            "Manage Roles",
            "Assign or remove permissions for a role."),

        new(
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000103"),
            Guid.Parse("9f4d8b1e-0000-0000-0000-000000000203"),
            Permissions.PermissionCatalog.View,
            "View Permissions",
            "View the catalog of available permissions."),
    ];

    public static ApplicationRole AdministratorRole { get; } = new()
    {
        Id = AdministratorRoleId,
        Name = "Administrator",
        NormalizedName = "ADMINISTRATOR",
        ConcurrencyStamp = "9f4d8b1e-0000-0000-0000-000000000002"
    };

    public static IReadOnlyList<object> PermissionSeed { get; } = Entries
        .Select(entry => (object)new
        {
            Id = entry.PermissionId,
            entry.Code,
            entry.Name,
            entry.Description,
            CreatedAt = SeededAt
        })
        .ToArray();

    public static IReadOnlyList<object> RolePermissionSeed { get; } = Entries
        .Select(entry => (object)new
        {
            Id = entry.RolePermissionId,
            RoleId = AdministratorRoleId,
            PermissionId = entry.PermissionId
        })
        .ToArray();

    private sealed record PermissionSeedEntry(
        Guid PermissionId,
        Guid RolePermissionId,
        string Code,
        string Name,
        string Description);
}
