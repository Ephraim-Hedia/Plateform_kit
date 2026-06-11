namespace Platform.Application.Authentication;

public sealed record AuthenticatedUser(Guid Id, string Email, IReadOnlyList<string> Roles);
