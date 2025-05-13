using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(p => p.TransactionId).IsRequired(false);
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodId);

            builder.HasOne(p => p.PaymentStatus)
                .WithMany(ps => ps.Payments)
                .HasForeignKey(p => p.StatusId);

            builder.HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId);

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
