using FluentValidation;

namespace Platform.Application.Roles.AssignPermissions;

public sealed class AssignPermissionsCommandValidator : AbstractValidator<AssignPermissionsCommand>
{
    public AssignPermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();

        RuleFor(x => x.PermissionCodes).NotNull();

        RuleForEach(x => x.PermissionCodes).NotEmpty();
    }
}
