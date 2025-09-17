using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        // Attributes
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever().IsRequired();
        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Seed data
        builder.HasData(GenerateRoles());
    }

    private static IEnumerable<EmployeeRole> GenerateRoles()
    {
        return Enum.GetValues<EmployeeRoleType>().Select(role => new EmployeeRole
        {
            Id = (int)role,
            Name = role.ToString()
        });
    }
}
