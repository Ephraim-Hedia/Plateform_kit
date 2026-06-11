using FluentAssertions;
using Platform.Domain.Entities;

namespace Platform.UnitTests.Domain.Entities;

public class RolePermissionTests
{
    [Fact]
    public void Create_Should_SetProperties()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var rolePermission = RolePermission.Create(roleId, permissionId);

        rolePermission.RoleId.Should().Be(roleId);
        rolePermission.PermissionId.Should().Be(permissionId);
        rolePermission.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Should_GenerateUniqueIds()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var first = RolePermission.Create(roleId, permissionId);
        var second = RolePermission.Create(roleId, permissionId);

        first.Id.Should().NotBe(second.Id);
    }
}
