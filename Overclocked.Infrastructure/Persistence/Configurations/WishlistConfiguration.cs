using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.ToTable("Wishlists");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => WishlistId.Create(value))
            .IsRequired();

        builder.Property(w => w.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Wishlist>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(w => w.WishlistItems, wib =>
        {
            wib.ToTable("WishlistItems");
            wib.WithOwner().HasForeignKey("WishlistId");

            wib.Property(wi => wi.ProductId)
                .HasColumnName("ProductId")
                .HasConversion(
                    id => id.Value,
                    value => ProductId.Create(value))
                .IsRequired();

            wib.HasKey("WishlistId", "ProductId");

            wib.HasOne<Product>()
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Indexes
        builder.HasIndex(w => w.UserId)
            .IsUnique();
    }
}
