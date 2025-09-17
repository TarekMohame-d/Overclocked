using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundItemConfiguration : IEntityTypeConfiguration<RefundItem>
{
    public void Configure(EntityTypeBuilder<RefundItem> builder)
    {
        // Attributes
        builder.HasKey(ri => ri.Id);
        builder.Property(ri => ri.Id).ValueGeneratedNever().IsRequired();
        builder.Property(ri => ri.RefundId).IsRequired();
        builder.Property(ri => ri.OrderItemId).IsRequired(false);
        builder.Property(ri => ri.InvoiceItemId).IsRequired(false);
        builder.Property(ri => ri.Quantity).IsRequired();
        builder.Property(ri => ri.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(ri => ri.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(ri => ri.Refund)
            .WithMany(r => r.RefundItems)
            .HasForeignKey(ri => ri.RefundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.OrderItem)
            .WithOne()
            .HasForeignKey<RefundItem>(ri => ri.OrderItemId);

        builder.HasOne(ri => ri.InvoiceItem)
            .WithOne()
            .HasForeignKey<RefundItem>(ri => ri.InvoiceItemId);

        // Indexes
        builder.HasIndex(ri => ri.RefundId);
    }
}
