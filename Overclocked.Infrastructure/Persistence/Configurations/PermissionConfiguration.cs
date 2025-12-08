using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.PermissionAggregate;
using Overclocked.Domain.PermissionAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Attributes
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => PermissionId.Create(value))
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique();

        // Seed data
        builder.HasData(GeneratePermissions());
    }

    private static IEnumerable<Permission> GeneratePermissions()
    {
        return Enum.GetValues<PermissionType>()
            .Select(permission => Permission.Create(PermissionId.Create((int)permission), permission.ToString()));
    }
}
