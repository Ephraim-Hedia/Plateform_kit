namespace Platform.Application.Abstractions;

public interface IPermissionCacheInvalidator
{
    void InvalidateRole(string roleName);
}
