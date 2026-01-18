using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");

        // Attributes
        builder.HasKey(c => c.Id);
        builder
            .Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => CartId.Create(value))
            .IsRequired();

        builder.Property(c => c.UserId).HasConversion(id => id.Value, value => UserId.Create(value)).IsRequired();

        // Relationships
        builder.HasOne<User>().WithOne().HasForeignKey<Cart>(c => c.UserId).OnDelete(DeleteBehavior.Cascade);

        ConfigureCartItems(builder);

        builder.Navigation(c => c.CartItems).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(c => c.UserId).IsUnique();
    }

    private static void ConfigureCartItems(EntityTypeBuilder<Cart> builder) =>
        builder.OwnsMany(
            c => c.CartItems,
            ciBuilder =>
            {
                ciBuilder.ToTable("cart_items");

                ciBuilder.WithOwner();

                ciBuilder.HasKey(ci => ci.Id);
                ciBuilder
                    .Property(ci => ci.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => CartItemId.Create(value))
                    .IsRequired();

                ciBuilder
                    .Property(ci => ci.ProductId)
                    .HasConversion(id => id.Value, value => ProductId.Create(value))
                    .IsRequired();

                ciBuilder.Property(ci => ci.Quantity).IsRequired();

                ciBuilder.Property(ci => ci.CreatedAt).HasColumnType("timestamptz").IsRequired();

                ciBuilder.Property(ci => ci.UpdatedAt).HasColumnType("timestamptz").IsRequired();

                ciBuilder.HasOne<Product>().WithMany().HasForeignKey(ci => ci.ProductId).OnDelete(DeleteBehavior.Restrict);
            }
        );
}
