using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EmployeeActivityLogConfigurations : IEntityTypeConfiguration<EmployeeActivityLog>
{
    public void Configure(EntityTypeBuilder<EmployeeActivityLog> builder)
    {
        // Attributes
        builder.HasKey(eal => eal.Id);
        builder.Property(eal => eal.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(eal => eal.EmployeeId)
            .IsRequired();
        builder.Property(eal => eal.Action)
            .IsRequired();

        builder.Property(eal => eal.CreatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(eal => eal.Employee)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(eal => eal.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(eal => eal.EmployeeId);

        builder.HasIndex(eal => eal.CreatedAt);
    }
}
