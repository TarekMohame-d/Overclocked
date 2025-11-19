using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Attributes
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).ValueGeneratedNever().IsRequired();
        builder.Property(oi => oi.OrderId).IsRequired();
        builder.Property(oi => oi.ProductId).IsRequired();
        builder.Property(oi => oi.ShipmentId).IsRequired(false);
        builder.Property(oi => oi.Shipped).HasDefaultValue(false);
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(8,2)").IsRequired();
        builder.Property(oi => oi.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
        builder.Property(oi => oi.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

        // Relationships
        builder
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(oi => oi.Shipment)
            .WithMany(s => s.ShipmentItems)
            .HasForeignKey(oi => oi.ShipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);
        builder.HasIndex(oi => oi.ShipmentId);
    }
}
