using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Attributes
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever().IsRequired();
        builder.Property(p => p.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(p => p.Name).IsUnique();

        // Seed data
        builder.HasData(GeneratePermissions());
    }

    private static IEnumerable<Permission> GeneratePermissions()
    {
        return Enum.GetValues<PermissionType>()
            .Select(role => new Permission { Id = (int)role, Name = role.ToString() });
    }
}
