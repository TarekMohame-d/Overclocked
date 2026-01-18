using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Configurations.Lookup;

public class PermissionConfiguration : IEntityTypeConfiguration<PermissionLookup>
{
    public void Configure(EntityTypeBuilder<PermissionLookup> builder)
    {
        builder.ToTable(
            "permissions",
            t =>
            {
                IEnumerable<int> validIds = Enum.GetValues<Permission>().Cast<int>();

                t.HasCheckConstraint("CK_Permissions_Id", $"id IN ({string.Join(", ", validIds)})");
            }
        );

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion<int>().ValueGeneratedNever().IsRequired();

        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();

        // Seed data
        builder.HasData(GeneratePermissions());
    }

    private static IEnumerable<PermissionLookup> GeneratePermissions() =>
        Enum.GetValues<Permission>().Select(permission => new PermissionLookup { Id = permission, Name = permission.ToString() });
}
