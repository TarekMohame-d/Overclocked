using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Authentication;

public class EmailConfirmationCodeConfiguration : IEntityTypeConfiguration<EmailConfirmationCode>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationCode> builder)
    {
        // Attributes
        builder.HasKey(ecc => ecc.UserId);
        builder.Property(ecc => ecc.UserId).ValueGeneratedNever().IsRequired();
        builder.Property(ecc => ecc.CodeHash).HasMaxLength(6).IsRequired();
        builder.Property(ecc => ecc.IsUsed).HasDefaultValue(false).IsRequired();
        builder.Property(ecc => ecc.ExpiredAt).HasColumnType("timestamptz").IsRequired();
        builder.Property(ecc => ecc.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(ecc => ecc.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(ecc => ecc.User)
            .WithOne(c => c.EmailConfirmationCode)
            .HasForeignKey<EmailConfirmationCode>(ecc => ecc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ecc => ecc.UserId)
            .IsUnique();
    }
}
