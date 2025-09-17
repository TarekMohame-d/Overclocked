using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundStatusConfiguration : IEntityTypeConfiguration<RefundStatus>
{
    public void Configure(EntityTypeBuilder<RefundStatus> builder)
    {
        // Attributes
        builder.HasKey(rs => rs.Id);
        builder.Property(rs => rs.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(rs => rs.Name)
            .IsUnique();

        // Seed Data
        builder.HasData(GenerateRefundStatus());
    }

    private IEnumerable<OrderStatus> GenerateRefundStatus()
    {
        return Enum.GetValues<RefundStatusType>().Select(role => new OrderStatus
        {
            Id = (int)role,
            Name = role.ToString(),
        });
    }
}
