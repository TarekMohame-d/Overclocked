using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FirstName).HasMaxLength(20).IsRequired();
            builder.Property(c => c.LastName).HasMaxLength(20).IsRequired();
            builder.Property(c => c.Email).HasMaxLength(100).IsRequired();
            builder.Property(c => c.HashPassword).HasMaxLength(256).IsRequired();
            builder.Property(c => c.Phone).HasMaxLength(20).IsRequired();
            builder.Property(c => c.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationships
            builder.HasMany(c => c.Addresses)
                   .WithOne(a => a.Customer)
                   .HasForeignKey(a => a.CustomerId);

            builder.HasOne(c => c.Cart)
                   .WithOne(cart => cart.Customer)
                   .HasForeignKey<Cart>(cart => cart.CustomerId);

            builder.HasMany(c => c.Orders)
                   .WithOne(order => order.Customer)
                   .HasForeignKey(order => order.CustomerId);

            builder.HasMany(c => c.Reviews)
                   .WithOne(review => review.Customer)
                   .HasForeignKey(review => review.CustomerId);

            builder.HasOne(c => c.Wishlist)
                   .WithOne(ws => ws.Customer)
                   .HasForeignKey<Wishlist>(ws => ws.CustomerId);

            builder.HasMany(c => c.Shipments)
                   .WithOne(s => s.Customer)
                   .HasForeignKey(s => s.CustomerId);

            builder.HasMany(c => c.Payments)
                   .WithOne(p => p.Customer)
                   .HasForeignKey(p => p.CustomerId);


            // Indexes
            builder.HasIndex(c => c.Email)
                   .IsUnique();
        }
    }
}
