using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        // Attributes
        builder.HasKey(os => os.Id);
        builder.Property(os => os.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(os => os.Name)
            .IsUnique();

        // Seed Data
        builder.HasData(GenerateOrderStatus());
    }

    private IEnumerable<OrderStatus> GenerateOrderStatus()
    {
        return Enum.GetValues<OrderStatusType>().Select(role => new OrderStatus
        {
            Id = (int)role,
            Name = role.ToString(),
        });
    }
}
