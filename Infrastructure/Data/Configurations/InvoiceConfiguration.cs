using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        // Attributes
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever().IsRequired();
        builder.Property(i => i.EmployeeId).IsRequired();
        builder.Property(i => i.CustomerName).HasMaxLength(50).IsRequired();
        builder.Property(i => i.CustomerPhone).HasMaxLength(25).IsRequired();
        builder.Property(i => i.TotalPrice).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(i => i.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(i => i.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(i => i.InvoiceStatusType);

        // Relationships
        builder.HasOne(i => i.Employee)
            .WithMany(e => e.Invoices)
            .HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.InvoiceStatus)
            .WithMany(iStatus => iStatus.Invoices)
            .HasForeignKey(i => i.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => i.EmployeeId);
        builder.HasIndex(i => i.CreatedAt);
        builder.HasIndex(i => i.CustomerPhone);
        builder.HasIndex(i => i.CustomerName);
    }
}
