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
        builder.ToTable("wishlists");

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

        ConfigureWishlistItems(builder);

        // Indexes
        builder.HasIndex(w => w.UserId)
            .IsUnique();
    }

    private static void ConfigureWishlistItems(EntityTypeBuilder<Wishlist> builder)
    {
        builder.OwnsMany(w => w.WishlistItems, wiBuilder =>
        {
            wiBuilder.ToTable("wishlist_items");

            wiBuilder.WithOwner().HasForeignKey("WishlistId");

            wiBuilder.Property<WishlistId>("WishlistId")
                .HasColumnName("wishlist_id");

            wiBuilder.Property(wi => wi.ProductId)
                .HasConversion(
                    id => id.Value,
                    value => ProductId.Create(value))
                .IsRequired();

            wiBuilder.HasKey("WishlistId", "ProductId");

            wiBuilder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
