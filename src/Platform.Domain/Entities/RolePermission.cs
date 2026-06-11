using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public class RolePermission : Entity<Guid>
{
    private RolePermission(Guid id, Guid roleId, Guid permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    private RolePermission()
    {
    }

    public Guid RoleId { get; private set; }

    public Guid PermissionId { get; private set; }

    public static RolePermission Create(Guid roleId, Guid permissionId) =>
        new(Guid.NewGuid(), roleId, permissionId);
}
