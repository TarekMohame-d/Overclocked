using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        // Attributes
        builder.HasKey(wi => wi.Id);
        builder.Property(wi => wi.Id).ValueGeneratedNever().IsRequired();
        builder.Property(wi => wi.WishlistId).IsRequired();
        builder.Property(wi => wi.ProductId).IsRequired();
        builder.Property(wi => wi.CreatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");
        builder.Property(wi => wi.UpdatedAt).HasColumnType("timestamptz").HasDefaultValueSql("NOW()");

        // Relationships
        builder
            .HasOne(wi => wi.Wishlist)
            .WithMany(w => w.WishlistItems)
            .HasForeignKey(wi => wi.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(wi => wi.Product)
            .WithMany(p => p.WishlistItems)
            .HasForeignKey(wi => wi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(wp => wp.WishlistId);
        builder.HasIndex(wp => wp.ProductId);
    }
}
