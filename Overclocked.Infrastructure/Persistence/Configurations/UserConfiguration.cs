using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // Attributes
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value))
            .IsRequired();

        builder.Property(u => u.Role)
            .HasColumnName("role_id")
            .HasConversion<int>()
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
        builder.HasOne<RoleLookup>()
            .WithMany()
            .HasForeignKey(u => u.Role)
            .IsRequired();

        ConfigureUserRefreshTokens(builder);

        ConfigureUserAddresses(builder);

        ConfigureUserEmailConfirmationCode(builder);

        builder.Navigation(u => u.RefreshTokens)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Addresses)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.EmailConfirmationCode)
            .AutoInclude(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.Phone)
            .IsUnique();
    }

    private static void ConfigureUserRefreshTokens(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(u => u.RefreshTokens, rtBuilder =>
        {
            rtBuilder.ToTable("refresh_tokens");

            rtBuilder.WithOwner(); // shadow property

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

            rtBuilder.HasIndex(rt => rt.DeviceId)
                .IsUnique();
        });
    }

    private static void ConfigureUserAddresses(EntityTypeBuilder<User> builder)
    {
        builder.OwnsMany(u => u.Addresses, uaBuilder =>
        {
            uaBuilder.ToTable("addresses");

            uaBuilder.WithOwner(); // shadow property

            uaBuilder.HasKey("Id");
            uaBuilder.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            uaBuilder.Property(a => a.City)
                .HasMaxLength(30)
                .IsRequired();

            uaBuilder.Property(a => a.Street)
                .HasMaxLength(100)
                .IsRequired();

            uaBuilder.Property(a => a.Description)
                .HasMaxLength(300)
                .IsRequired();

            uaBuilder.Property(a => a.PostalCode)
                .HasMaxLength(10)
                .IsRequired();

            uaBuilder.Property(a => a.IsDeleted)
                .IsRequired();

            uaBuilder.HasIndex(a => a.City);
        });
    }

    private static void ConfigureUserEmailConfirmationCode(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.EmailConfirmationCode, uEmail =>
        {
            uEmail.ToTable("email_confirmation_codes");

            uEmail.WithOwner().HasForeignKey("UserId");

            uEmail.Property<UserId>("UserId")
                .HasColumnName("user_id");

            uEmail.HasKey("UserId");

            uEmail.Property(ecc => ecc.CodeHash)
                    .IsRequired();

            uEmail.Property(ecc => ecc.IsUsed)
                .IsRequired();

            uEmail.Property(ecc => ecc.ExpiredAt)
                .HasColumnType("timestamptz")
                .IsRequired();
        });
    }
}
