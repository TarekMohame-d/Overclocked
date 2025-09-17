using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Attributes
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever().IsRequired();
        builder.Property(e => e.Username).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FirstName).HasMaxLength(20).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(20).IsRequired();
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.Phone).HasMaxLength(25).IsRequired();
        builder.Property(e => e.RoleId).IsRequired();
        builder.Property(e => e.Email).IsRequired(false);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(b => b.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(b => b.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(e => e.RoleType);

        // Relationships
        builder.HasOne(e => e.Role)
            .WithMany(r => r.Employees)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.Username)
            .IsUnique();
        builder.HasIndex(e => e.RoleId);
        builder.HasIndex(e => e.IsActive);
    }
}
