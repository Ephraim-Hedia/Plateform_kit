using Microsoft.AspNetCore.Authorization;

namespace Platform.Infrastructure.Authorization;

public sealed class HasPermissionAttribute(string permission)
    : AuthorizeAttribute(PermissionAuthorizationPolicyProvider.PolicyPrefix + permission);
