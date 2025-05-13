using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);

            builder.HasOne(w => w.Customer)
                   .WithOne(c => c.Wishlist)
                   .HasForeignKey<Wishlist>(w => w.CustomerId);

            builder.HasMany(w => w.WishlistProducts)
                   .WithOne(wp => wp.Wishlist)
                   .HasForeignKey(wp => wp.WishlistId);
        }
    }
}
