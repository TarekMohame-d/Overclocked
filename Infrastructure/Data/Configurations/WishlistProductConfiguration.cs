using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class WishlistProductConfiguration : IEntityTypeConfiguration<WishlistProduct>
    {
        public void Configure(EntityTypeBuilder<WishlistProduct> builder)
        {
            builder.HasKey(wp => new { wp.WishlistId, wp.ProductId });

            builder.HasOne(wp => wp.Wishlist)
                   .WithMany(w => w.WishlistProducts)
                   .HasForeignKey(wp => wp.WishlistId);

            builder.HasOne(wp => wp.Product)
                   .WithMany(p => p.WishlistProducts)
                   .HasForeignKey(wp => wp.ProductId);
        }
    }
}
