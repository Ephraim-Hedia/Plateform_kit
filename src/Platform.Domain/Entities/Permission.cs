using Platform.Domain.Common;

namespace Platform.Domain.Entities;

public class Permission : AuditableEntity<Guid>
{
    private Permission(Guid id, string code, string name, string? description)
        : base(id)
    {
        Code = code;
        Name = name;
        Description = description;
    }

    private Permission()
    {
    }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public static Permission Create(string code, string name, string? description = null) =>
        new(Guid.NewGuid(), code, name, description);
}
