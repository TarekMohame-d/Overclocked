using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Configurations.Lookup;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionLookup>
{
    public void Configure(EntityTypeBuilder<RolePermissionLookup> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Property(rp => rp.RoleId).HasColumnName("role_id").HasConversion<int>();

        builder.Property(rp => rp.PermissionId).HasColumnName("permission_id").HasConversion<int>();

        builder.HasOne<RoleLookup>().WithMany().HasForeignKey(rp => rp.RoleId);

        builder.HasOne<PermissionLookup>().WithMany().HasForeignKey(rp => rp.PermissionId);

        builder.HasData(GenerateRolePermissions());
    }

    private static IEnumerable<RolePermissionLookup> GenerateRolePermissions()
    {
        yield return CreateLink(Role.DataEntry, Permission.AddEditDelete);

        foreach (Permission permission in Enum.GetValues<Permission>())
        {
            yield return CreateLink(Role.SuperAdmin, permission);
            if (permission >= Permission.ManageOrders)
                yield return CreateLink(Role.Admin, permission);
        }
    }

    private static RolePermissionLookup CreateLink(Role role, Permission permission) =>
        new() { RoleId = role, PermissionId = permission };
}
