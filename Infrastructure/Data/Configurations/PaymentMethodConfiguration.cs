using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        // Attributes
        builder.HasKey(PM => PM.Id);
        builder.Property(PM => PM.Id).ValueGeneratedNever().IsRequired();
        builder.Property(PM => PM.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(PM => PM.Name).IsUnique();

        // Seed Data
        builder.HasData(GeneratePaymentMethod());
    }

    private IEnumerable<PaymentMethod> GeneratePaymentMethod()
    {
        return Enum.GetValues<PaymentMethodType>()
            .Select(role => new PaymentMethod { Id = (int)role, Name = role.ToString() });
    }
}
