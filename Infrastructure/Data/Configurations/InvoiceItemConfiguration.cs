using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        // Attributes
        builder.HasKey(ii => ii.Id);
        builder.Property(ii => ii.Id).ValueGeneratedNever().IsRequired();
        builder.Property(ii => ii.InvoiceId).IsRequired();
        builder.Property(ii => ii.ProductId).IsRequired();
        builder.Property(ii => ii.Quantity).IsRequired();
        builder.Property(ii => ii.UnitPrice).HasColumnType("decimal(8,2)").IsRequired();
        builder.Property(ii => ii.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(ii => ii.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(ii => ii.Invoice)
            .WithMany(i => i.InvoiceItems)
            .HasForeignKey(ii => ii.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ii => ii.Product)
            .WithMany(p => p.InvoiceItems)
            .HasForeignKey(ii => ii.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(oi => oi.InvoiceId);
        builder.HasIndex(oi => oi.ProductId);
    }
}
