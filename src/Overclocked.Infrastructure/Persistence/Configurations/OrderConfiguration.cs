using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(
            "orders",
            t =>
            {
                IEnumerable<int> validIds = Enum.GetValues<OrderStatus>().Cast<int>();
                t.HasCheckConstraint("CK_Orders_Status", $"status_id IN ({string.Join(", ", validIds)})");
            }
        );

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasConversion(id => id.Value, value => OrderId.Create(value)).IsRequired();

        builder.Property(o => o.UserId).HasConversion(id => id.Value, value => UserId.Create(value)).IsRequired();

        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();

        builder.Property(o => o.Status).HasConversion<int>().HasColumnName("status_id").IsRequired();

        builder.OwnsOne(o => o.ShippingAddress, address => address.ToJson());

        builder.Property(s => s.CreatedAt).IsRequired();

        builder.Property(s => s.UpdatedAt).IsRequired();

        ConfigureOrderTotalPrice(builder);

        ConfigureOrderItems(builder);

        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
    }

    private static void ConfigureOrderTotalPrice(EntityTypeBuilder<Order> builder) =>
        builder.OwnsOne(
            o => o.TotalPrice,
            price =>
            {
                price.Property(m => m.Value).HasColumnName("total_price_amount").HasPrecision(8, 2).IsRequired();

                price.Property(m => m.Currency).HasColumnName("total_price_currency").HasMaxLength(3).IsRequired();
            }
        );

    private static void ConfigureOrderItems(EntityTypeBuilder<Order> builder) =>
        builder.OwnsMany(
            o => o.Items,
            orderItemBuilder =>
            {
                orderItemBuilder.ToTable("order_items");

                orderItemBuilder.WithOwner();

                orderItemBuilder.HasKey(oi => oi.Id);
                orderItemBuilder
                    .Property(oi => oi.Id)
                    .ValueGeneratedNever()
                    .HasConversion(id => id.Value, value => OrderItemId.Create(value))
                    .IsRequired();

                orderItemBuilder
                    .Property(t => t.ProductId)
                    .HasConversion(id => id.Value, value => ProductId.Create(value))
                    .IsRequired();

                orderItemBuilder.Property(oi => oi.ProductName).HasMaxLength(50).IsRequired();

                orderItemBuilder
                    .Property(oi => oi.ProductImage)
                    .HasConversion(name => name.Value, value => Image.Load(value))
                    .IsRequired();

                orderItemBuilder.OwnsOne(
                    oi => oi.UnitPrice,
                    price =>
                    {
                        price.Property(m => m.Value).HasColumnName("unit_price").HasPrecision(8, 2).IsRequired();

                        price.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
                    }
                );

                orderItemBuilder.Property(oi => oi.Quantity).IsRequired();

                orderItemBuilder.Property(oi => oi.CreatedAt).IsRequired();

                orderItemBuilder.Property(oi => oi.UpdatedAt).IsRequired();
            }
        );
}
