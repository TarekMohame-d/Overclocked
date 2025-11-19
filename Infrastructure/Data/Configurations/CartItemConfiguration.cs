using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        // Attributes
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Id).ValueGeneratedNever().IsRequired();
        builder.Property(ci => ci.CartId).IsRequired();
        builder.Property(ci => ci.ProductId).IsRequired();
        builder.Property(ci => ci.Quantity).IsRequired();
        builder.Property(ci => ci.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
        builder.Property(ci => ci.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

        // Relationships
        builder
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ci => ci.CartId);
        builder.HasIndex(ci => ci.ProductId);
    }
}
