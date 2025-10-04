using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // Attributes
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.Property(rp => rp.PermissionId).IsRequired();
        builder.Property(rp => rp.RoleId).IsRequired();

        // Relationships
        builder.HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rp => rp.PermissionId);
        builder.HasIndex(rp => rp.RoleId);

        // Seed data
        builder.HasData(GeneratePermissions());
    }

    private static List<RolePermission> GeneratePermissions()
    {
        var allPermissions = Enum.GetValues<PermissionType>().Cast<PermissionType>().ToList();

        // Role IDs from enum
        const int SuperAdmin = 1;
        const int Admin = 2;
        const int DataEntry = 3;

        var list = new List<RolePermission>();

        // SuperAdmin
        list.AddRange(allPermissions.Select(p => new RolePermission
        {
            RoleId = SuperAdmin,
            PermissionId = (int)p
        }));

        // Admin
        list.AddRange(allPermissions
            .Where(p => p > PermissionType.DeactivateUsers)
            .Select(p => new RolePermission
            {
                RoleId = Admin,
                PermissionId = (int)p
            }));

        // DataEntry
        list.Add(
            new RolePermission
            {
                RoleId = DataEntry,
                PermissionId = (int)PermissionType.AddEditDelete
            });

        return list;
    }
}
