using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Attributes
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Seed data
        builder.HasData(GenerateRoles());
    }

    private static IEnumerable<Role> GenerateRoles()
    {
        return Enum.GetValues<RoleType>()
            .Select(role => new Role { Id = (int)role, Name = role.ToString() });
    }
}
