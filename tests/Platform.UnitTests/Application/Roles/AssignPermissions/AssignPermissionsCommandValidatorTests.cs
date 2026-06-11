using FluentAssertions;
using Platform.Application.Roles.AssignPermissions;

namespace Platform.UnitTests.Application.Roles.AssignPermissions;

public class AssignPermissionsCommandValidatorTests
{
    private readonly AssignPermissionsCommandValidator _validator = new();

    [Fact]
    public void Validate_Should_Succeed_WhenCommandIsValid()
    {
        var result = _validator.Validate(new AssignPermissionsCommand(Guid.NewGuid(), ["Roles.View"]));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_Fail_WhenRoleIdIsEmpty()
    {
        var result = _validator.Validate(new AssignPermissionsCommand(Guid.Empty, ["Roles.View"]));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AssignPermissionsCommand.RoleId));
    }

    [Fact]
    public void Validate_Should_Fail_WhenPermissionCodesIsNull()
    {
        var result = _validator.Validate(new AssignPermissionsCommand(Guid.NewGuid(), null!));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(AssignPermissionsCommand.PermissionCodes));
    }

    [Fact]
    public void Validate_Should_Fail_WhenPermissionCodeIsEmpty()
    {
        var result = _validator.Validate(new AssignPermissionsCommand(Guid.NewGuid(), [""]));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PermissionCodes[0]");
    }

    [Fact]
    public void Validate_Should_Succeed_WhenPermissionCodesIsEmptyList()
    {
        var result = _validator.Validate(new AssignPermissionsCommand(Guid.NewGuid(), []));

        result.IsValid.Should().BeTrue();
    }
}
