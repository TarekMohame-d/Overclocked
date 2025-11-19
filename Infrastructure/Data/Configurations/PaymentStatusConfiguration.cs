using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentStatusConfiguration : IEntityTypeConfiguration<PaymentStatus>
{
    public void Configure(EntityTypeBuilder<PaymentStatus> builder)
    {
        // Attributes
        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id).ValueGeneratedNever().IsRequired();
        builder.Property(ps => ps.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(ps => ps.Name).IsUnique();

        // Seed Data
        builder.HasData(GeneratePaymentStatus());
    }

    private IEnumerable<PaymentStatus> GeneratePaymentStatus()
    {
        return Enum.GetValues<PaymentStatusType>()
            .Select(role => new PaymentStatus { Id = (int)role, Name = role.ToString() });
    }
}
