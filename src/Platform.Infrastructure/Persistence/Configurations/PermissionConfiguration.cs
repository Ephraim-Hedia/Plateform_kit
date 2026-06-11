using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Platform.Domain.Entities;
using Platform.Infrastructure.Persistence.Seed;

namespace Platform.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(permission => permission.Code).IsUnique();

        builder.Property(permission => permission.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(permission => permission.Description)
            .HasMaxLength(500);

        builder.HasData(AuthorizationSeedData.PermissionSeed);
    }
}
