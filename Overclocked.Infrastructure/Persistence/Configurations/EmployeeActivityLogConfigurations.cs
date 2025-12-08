using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.EmployeeActivityLogAggregate;
using Overclocked.Domain.EmployeeActivityLogAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class EmployeeActivityLogConfigurations : IEntityTypeConfiguration<EmployeeActivityLog>
{
    public void Configure(EntityTypeBuilder<EmployeeActivityLog> builder)
    {
        builder.ToTable("EmployeeActivityLogs");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => EmployeeActivityLogId.Create(value))
            .IsRequired();

        builder.Property(e => e.EmployeeId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(e => e.Action)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        // Relationships
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EmployeeId);
    }
}
