using Domain.Entities;
using Domain.StaticData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Attributes
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever().IsRequired();
        builder.Property(u => u.RoleId).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(20).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
        builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Phone).HasMaxLength(25).IsRequired();
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .ValueGeneratedOnAdd();
        builder.Property(u => u.CreatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");
        builder.Property(u => u.UpdatedAt).HasColumnType("timestamptz")
            .HasDefaultValueSql("NOW()");

        builder.Ignore(u => u.RoleType);

        // Relationships
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();
        builder.HasIndex(u => u.Phone)
            .IsUnique();
        builder.HasIndex(u => u.RoleId);

        // Seed Data
        builder.HasData(SeedingAdmin());
    }

    private static User SeedingAdmin()
    {
        return new User
        {
            Id = Guid.Parse("019a497f-e294-71ac-8f28-6f772f4289e1"),
            RoleType = RoleType.SuperAdmin,
            FirstName = "Super",
            LastName = "Admin",
            Email = "overclocked.cor@gmail.com",
            EmailConfirmed = true,
            PasswordHash = "83F0B98915AA027B1D0A55E018181ACC2BDD9088F085A9832CE2081337BC4743-42C24EE7A22304068F0F8745D27B3C38",
            IsActive = true,
            Phone = "011xxxxxx24"
        };
    }
}
