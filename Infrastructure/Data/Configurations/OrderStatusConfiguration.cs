using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasKey(os => os.OrderStatusId);
            builder.Property(os => os.OrderStatusName).HasMaxLength(200).IsRequired();

            builder.HasMany(os => os.Orders)
                   .WithOne(o => o.OrderStatus)
                   .HasForeignKey(o => o.StatusId);

            // Indexes
            builder.HasIndex(os => os.OrderStatusName).IsUnique();
        }
    }
}
