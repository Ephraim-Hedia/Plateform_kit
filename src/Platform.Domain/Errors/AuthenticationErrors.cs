namespace Platform.Domain.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Authentication.InvalidCredentials",
        "The email or password is incorrect.");

    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "Authentication.EmailAlreadyExists",
        "An account with this email already exists.");

    public static readonly Error InvalidRefreshToken = Error.Unauthorized(
        "Authentication.InvalidRefreshToken",
        "The refresh token is invalid or has expired.");

    public static readonly Error UserNotFound = Error.NotFound(
        "Authentication.UserNotFound",
        "The user could not be found.");
}
