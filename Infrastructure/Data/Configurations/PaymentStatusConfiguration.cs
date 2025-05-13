using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PaymentStatusConfiguration : IEntityTypeConfiguration<PaymentStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentStatus> builder)
        {
            builder.HasKey(ps => ps.PaymentStatusId);
            builder.Property(ps => ps.PaymentStatusName).HasMaxLength(200).IsRequired();

            builder.HasMany(ps => ps.Payments)
                   .WithOne(p => p.PaymentStatus)
                   .HasForeignKey(p => p.StatusId);

            // Indexes
            builder.HasIndex(ps => ps.PaymentStatusName).IsUnique();
        }
    }
}
