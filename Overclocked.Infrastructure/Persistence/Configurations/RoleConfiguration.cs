using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.PermissionAggregate;
using Overclocked.Domain.PermissionAggregate.ValueObjects;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Attributes
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value))
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Relationships
        builder.OwnsMany(r => r.RolePermissions, rp =>
        {
            rp.ToTable("RolePermissions");

            rp.WithOwner().HasForeignKey("RoleId"); // shadow property

            rp.Property<RoleId>("RoleId")
                .HasColumnName("RoleId")
                .HasConversion(
                    id => id.Value,
                    value => RoleId.Create(value));

            rp.Property(rp => rp.PermissionId)
                .HasColumnName("PermissionId")
                .HasConversion(
                    id => id.Value,
                    value => PermissionId.Create(value))
                .IsRequired();

            rp.HasKey("RoleId", "PermissionId");

            rp.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            rp.HasData(GenerateRolePermissions());
        });

        builder.Navigation(rp => rp.RolePermissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Seed data
        builder.HasData(GenerateRoles());
    }

    private static IEnumerable<Role> GenerateRoles()
    {
        return Enum.GetValues<RoleType>()
            .Select(role => Role.Create(RoleId.Create((int)role), role.ToString()));
    }

    private static List<object> GenerateRolePermissions()
    {
        List<PermissionType> allPermissions = Enum.GetValues<PermissionType>().ToList();
        var seedData = new List<object>();

        // Super Admin
        foreach(PermissionType p in allPermissions)
        {
            seedData.Add(new
            {
                RoleId = RoleId.Create((int)RoleType.SuperAdmin),
                PermissionId = PermissionId.Create((int)p)
            });
        }

        // Admin
        IEnumerable<PermissionType> adminPermissions = allPermissions.Where(p => p > PermissionType.DeactivateUsers);
        foreach(PermissionType p in adminPermissions)
        {
            seedData.Add(new
            {
                RoleId = RoleId.Create((int)RoleType.Admin),
                PermissionId = PermissionId.Create((int)p)
            });
        }

        // Data Entry
        seedData.Add(new
        {
            RoleId = RoleId.Create((int)RoleType.DataEntry),
            PermissionId = PermissionId.Create((int)PermissionType.AddEditDelete)
        });

        return seedData;
    }
}
