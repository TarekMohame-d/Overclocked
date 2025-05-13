using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(PM => PM.PaymentMethodId);
            builder.Property(PM => PM.PaymentMethodName).HasMaxLength(100).IsRequired();

            builder.HasMany(PM => PM.Payments)
                .WithOne(P => P.PaymentMethod)
                .HasForeignKey(P => P.PaymentMethodId);

            // Indexes
            builder.HasIndex(PM => PM.PaymentMethodName).IsUnique();
        }
    }
}
