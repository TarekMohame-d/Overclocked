using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Attributes
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever().IsRequired();
        builder.Property(p => p.MethodId).IsRequired();
        builder.Property(p => p.StatusId).IsRequired();
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.Amount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(p => p.TransactionId).IsRequired(false);
        builder.Property(p => p.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(p => p.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(p => p.PaymentStatusType);
        builder.Ignore(p => p.PaymentMethodType);

        // Relationships
        builder.HasOne(p => p.PaymentMethod)
            .WithMany(pm => pm.Payments)
            .HasForeignKey(p => p.MethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PaymentStatus)
            .WithMany(ps => ps.Payments)
            .HasForeignKey(p => p.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.MethodId);
        builder.HasIndex(p => p.StatusId);
        builder.HasIndex(p => p.Amount);
    }
}
