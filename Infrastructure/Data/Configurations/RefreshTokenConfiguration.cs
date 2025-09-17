using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Authentication;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Attributes
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id).ValueGeneratedNever().IsRequired();
        builder.Property(rt => rt.DeviceId).IsRequired();
        builder.Property(rt => rt.DeviceType).HasMaxLength(20).IsRequired();
        builder.Property(rt => rt.UserId).IsRequired(false);
        builder.Property(rt => rt.EmployeeId).IsRequired(false);
        builder.Property(rt => rt.TokenHash).IsRequired();
        builder.Property(rt => rt.ExpiredAt).HasColumnType("timestamptz").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rt => rt.Employee)
            .WithMany(e => e.RefreshTokens)
            .HasForeignKey(rt => rt.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(rt => new { rt.UserId, rt.DeviceId })
            .IsUnique();
        builder.HasIndex(rt => rt.DeviceId);
    }
}
