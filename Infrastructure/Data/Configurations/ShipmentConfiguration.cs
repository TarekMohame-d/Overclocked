using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        // Attributes
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever().IsRequired();
        builder.Property(s => s.OrderId).IsRequired();
        builder.Property(s => s.AddressId).IsRequired();
        builder.Property(s => s.StatusId).IsRequired();
        builder.Property(s => s.CarrierName).HasMaxLength(50).IsRequired();
        builder.Property(s => s.TrackingNumber).IsRequired();
        builder.Property(s => s.ShippedAt).HasColumnType("timestamptz")
            .IsRequired(false);
        builder.Property(s => s.EstimatedDeliveryDate).HasColumnType("timestamptz")
            .IsRequired();
        builder.Property(s => s.DeliveredAt).HasColumnType("timestamptz")
            .IsRequired(false);
        builder.Property(s => s.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(s => s.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(s => s.ShipmentStatusType);

        // Relationships
        builder.HasOne(s => s.Address)
            .WithMany(a => a.Shipments)
            .HasForeignKey(s => s.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Order)
            .WithMany(o => o.Shipments)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ShipmentStatus)
            .WithMany(ss => ss.Shipments)
            .HasForeignKey(s => s.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.OrderId);
        builder.HasIndex(s => s.AddressId);
        builder.HasIndex(s => s.CarrierName);
        builder.HasIndex(s => s.StatusId);
    }
}
