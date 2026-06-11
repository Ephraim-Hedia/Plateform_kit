namespace Platform.Application.Permissions.GetPermissions;

public sealed record PermissionDto(Guid Id, string Code, string Name, string? Description);
