using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => CartId.Create(value))
            .IsRequired();

        builder.Property(c => c.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(c => c.CartItems, cib =>
        {
            cib.ToTable("CartItems");

            cib.WithOwner().HasForeignKey("CartId");

            cib.HasKey(ci => ci.Id);
            cib.Property(ci => ci.Id)
                .ValueGeneratedNever()
                    .HasConversion(
                        id => id.Value,
                        value => CartItemId.Create(value))
                    .IsRequired();

            cib.Property(ci => ci.ProductId)
                .HasConversion(
                    id => id.Value,
                    value => ProductId.Create(value))
                    .IsRequired();

            cib.Property(ci => ci.Quantity)
                .IsRequired();

            cib.Property(ci => ci.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            cib.Property(ci => ci.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();
        });

        builder.Navigation(c => c.CartItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(c => c.UserId)
            .IsUnique();
    }
}
