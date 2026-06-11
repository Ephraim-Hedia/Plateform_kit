using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Entities;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Persistence.Seed;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rolePermission => rolePermission.Id);

        builder.HasIndex(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId })
            .IsUnique();

        builder.HasOne<ApplicationRole>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(AuthorizationSeedData.RolePermissionSeed);
    }
}
