using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Attributes
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(u => u.RoleId)
            .HasConversion(
                id => id.Value,
                value => RoleId.Create(value))
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.EmailConfirmed)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        // Relationships
        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(u => u.RefreshTokens, rtBuilder =>
        {
            rtBuilder.ToTable("RefreshTokens");

            rtBuilder.WithOwner().HasForeignKey("UserId"); // shadow property

            rtBuilder.HasKey(rt => rt.Id);
            rtBuilder.Property(rt => rt.Id)
                .HasConversion(
                    id => id.Value,
                    value => RefreshTokenId.Create(value))
                .IsRequired();

            rtBuilder.Property(rt => rt.DeviceId)
                .IsRequired();

            rtBuilder.Property(rt => rt.TokenHash)
                .IsRequired();

            rtBuilder.Property(rt => rt.ExpiredAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            rtBuilder.Property(rt => rt.CreatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            rtBuilder.Property(rt => rt.UpdatedAt)
                .HasColumnType("timestamptz")
                .IsRequired();

            rtBuilder.HasIndex(rt => rt.DeviceId);
        });

        builder.OwnsMany(u => u.Addresses, ua =>
        {
            ua.ToTable("Addresses");

            ua.WithOwner().HasForeignKey("UserId"); // shadow property

            ua.HasKey("Id");
            ua.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            ua.Property(a => a.City)
                .HasMaxLength(30)
                .IsRequired();

            ua.Property(a => a.Street)
                .HasMaxLength(100)
                .IsRequired();

            ua.Property(a => a.Description)
                .HasMaxLength(300)
                .IsRequired();

            ua.HasIndex(a => a.City);
        });

        builder.OwnsOne(u => u.EmailConfirmationCode, uEmail =>
        {
            uEmail.ToTable("EmailConfirmationCodes");

            uEmail.WithOwner().HasForeignKey("UserId"); // shadow property

            uEmail.HasKey("UserId");

            uEmail.Property(ecc => ecc.CodeHash)
                    .IsRequired();

            uEmail.Property(ecc => ecc.IsUsed)
                .IsRequired();

            uEmail.Property(ecc => ecc.ExpiredAt)
                .HasColumnType("timestamptz")
                .IsRequired();
        });

        builder.Navigation(u => u.RefreshTokens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Addresses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Phone)
            .IsUnique();

        // Seed Data
        // builder.HasData(SeedingAdmin());
    }

    // private static User SeedingAdmin()
    // {
    //     return new User
    //     {
    //         Id = Guid.Parse("019a497f-e294-71ac-8f28-6f772f4289e1"),
    //         RoleType = RoleType.SuperAdmin,
    //         FirstName = "Super",
    //         LastName = "Admin",
    //         Email = "overclocked.cor@gmail.com",
    //         EmailConfirmed = true,
    //         PasswordHash =
    //             "83F0B98915AA027B1D0A55E018181ACC2BDD9088F085A9832CE2081337BC4743-42C24EE7A22304068F0F8745D27B3C38",
    //         IsActive = true,
    //         Phone = "011xxxxxx24",
    //     };
    // }
}
