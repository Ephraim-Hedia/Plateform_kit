namespace Platform.Application.Roles.GetRoles;

public sealed record RoleDto(Guid Id, string Name, IReadOnlyList<string> Permissions);
