using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        // Attributes
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever().IsRequired();
        builder.Property(r => r.EmployeeId).IsRequired();
        builder.Property(r => r.StatusId).IsRequired();
        builder.Property(r => r.InvoiceId).IsRequired(false);
        builder.Property(r => r.OrderId).IsRequired(false);
        builder.Property(r => r.RefundAmount).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(r => r.RefundReason).HasMaxLength(100).IsRequired();
        builder.Property(r => r.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(r => r.RefundStatus);

        // Relationships
        builder.HasOne(r => r.Employee)
            .WithMany(e => e.Refunds)
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Order)
            .WithOne(o => o.Refund)
            .HasForeignKey<Refund>(r => r.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Invoice)
            .WithMany(i => i.Refunds)
            .HasForeignKey(r => r.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RefundStatus)
            .WithMany(rs => rs.Refunds)
            .HasForeignKey(r => r.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.EmployeeId);
        builder.HasIndex(r => r.StatusId);
        builder.HasIndex(r => r.OrderId);
    }
}
