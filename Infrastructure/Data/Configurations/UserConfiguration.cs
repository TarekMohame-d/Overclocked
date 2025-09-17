using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Attributes
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever().IsRequired();
        builder.Property(c => c.FirstName).HasMaxLength(20).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(100).IsRequired();
        builder.Property(c => c.EmailConfirmed).HasDefaultValue(false);
        builder.Property(c => c.PasswordHash).IsRequired();
        builder.Property(c => c.Phone).HasMaxLength(25).IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();
        builder.HasIndex(u => u.Phone)
            .IsUnique();
    }
}
