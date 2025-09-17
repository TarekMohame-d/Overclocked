using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
    public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
    {
        // Attributes
        builder.HasKey(iStatus => iStatus.Id);
        builder.Property(iStatus => iStatus.Id).ValueGeneratedNever().IsRequired();
        builder.Property(iStatus => iStatus.Name).HasMaxLength(50).IsRequired();

        // Relationships

        // Indexes
        builder.HasIndex(iStatus => iStatus.Name)
            .IsUnique();

        // Seed Data
        builder.HasData(GenerateInvoiceStatus());
    }

    private IEnumerable<InvoiceStatus> GenerateInvoiceStatus()
    {
        return Enum.GetValues<InvoiceStatusType>().Select(role => new InvoiceStatus
        {
            Id = (int)role,
            Name = role.ToString(),
        });
    }
}

