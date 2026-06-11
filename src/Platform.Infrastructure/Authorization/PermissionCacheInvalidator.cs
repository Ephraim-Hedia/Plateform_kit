using Microsoft.Extensions.Caching.Memory;
using Platform.Application.Abstractions;

namespace Platform.Infrastructure.Authorization;

public sealed class PermissionCacheInvalidator(IMemoryCache cache) : IPermissionCacheInvalidator
{
    public void InvalidateRole(string roleName) => cache.Remove(PermissionCacheKeys.ForRole(roleName));
}
