namespace Platform.Domain.Errors;

public static class AuthorizationErrors
{
    public static readonly Error RoleNotFound = Error.NotFound(
        "Authorization.RoleNotFound",
        "The role could not be found.");

    public static readonly Error PermissionNotFound = Error.NotFound(
        "Authorization.PermissionNotFound",
        "One or more permissions could not be found.");
}
