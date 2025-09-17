using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ShipmentStatusConfiguration : IEntityTypeConfiguration<ShipmentStatus>
{
    public void Configure(EntityTypeBuilder<ShipmentStatus> builder)
    {
        // Attributes
        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Id).ValueGeneratedNever().IsRequired();
        builder.Property(ss => ss.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(ss => ss.Name)
            .IsUnique();

        // Seed Data
        builder.HasData(GenerateShipmentStatus());
    }

    private IEnumerable<ShipmentStatus> GenerateShipmentStatus()
    {
        return Enum.GetValues<ShipmentStatusType>().Select(role => new ShipmentStatus
        {
            Id = (int)role,
            Name = role.ToString(),
        });
    }
}
