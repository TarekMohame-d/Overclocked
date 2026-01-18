using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Configurations.Lookup;

public class RoleConfiguration : IEntityTypeConfiguration<RoleLookup>
{
    public void Configure(EntityTypeBuilder<RoleLookup> builder)
    {
        builder.ToTable(
            "roles",
            t =>
            {
                IEnumerable<int> validIds = Enum.GetValues<Role>().Cast<int>();

                t.HasCheckConstraint("CK_Roles_Id", $"id IN ({string.Join(", ", validIds)})");
            }
        );

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasConversion<int>().ValueGeneratedNever().IsRequired();

        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();

        // Seed data
        builder.HasData(GenerateRoles());
    }

    private static IEnumerable<RoleLookup> GenerateRoles() =>
        Enum.GetValues<Role>().Select(role => new RoleLookup { Id = role, Name = role.ToString() });
}
