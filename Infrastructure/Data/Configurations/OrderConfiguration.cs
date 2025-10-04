using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Attributes
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever().IsRequired();
        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.StatusId).IsRequired();
        builder.Property(o => o.ShippingCost).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(o => o.TotalPrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(o => o.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(o => o.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(o => o.OrderStatusType);

        // Relationships
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.OrderStatus)
            .WithMany(os => os.Orders)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.StatusId);
    }
}
