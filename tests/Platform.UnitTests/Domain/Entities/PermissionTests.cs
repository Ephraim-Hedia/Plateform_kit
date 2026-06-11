using FluentAssertions;
using Platform.Domain.Entities;

namespace Platform.UnitTests.Domain.Entities;

public class PermissionTests
{
    [Fact]
    public void Create_Should_SetProperties()
    {
        var permission = Permission.Create("Roles.View", "View Roles", "Allows viewing roles.");

        permission.Code.Should().Be("Roles.View");
        permission.Name.Should().Be("View Roles");
        permission.Description.Should().Be("Allows viewing roles.");
        permission.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_Should_AllowNullDescription()
    {
        var permission = Permission.Create("Roles.View", "View Roles");

        permission.Description.Should().BeNull();
    }

    [Fact]
    public void Create_Should_GenerateUniqueIds()
    {
        var first = Permission.Create("Roles.View", "View Roles");
        var second = Permission.Create("Roles.View", "View Roles");

        first.Id.Should().NotBe(second.Id);
    }
}
