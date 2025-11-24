using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<Permission> Permissions { get; set; }
    public required DbSet<RolePermission> RolePermissions { get; set; }
    public required DbSet<Role> Roles { get; set; }
    public required DbSet<EmployeeActivityLog> EmployeeActivityLogs { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }
    public required DbSet<EmailConfirmationCode> EmailConfirmationCodes { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Address> Addresses { get; set; }
    public required DbSet<Shipment> Shipments { get; set; }
    public required DbSet<ShipmentStatus> ShipmentStatuses { get; set; }
    public required DbSet<Order> Orders { get; set; }
    public required DbSet<OrderItem> OrderItems { get; set; }
    public required DbSet<OrderStatus> OrderStatuses { get; set; }
    public required DbSet<Payment> Payments { get; set; }
    public required DbSet<PaymentMethod> PaymentMethods { get; set; }
    public required DbSet<PaymentStatus> PaymentStatuses { get; set; }
    public required DbSet<Cart> Carts { get; set; }
    public required DbSet<CartItem> CartItems { get; set; }
    public required DbSet<Wishlist> Wishlists { get; set; }
    public required DbSet<WishlistItem> WishlistItems { get; set; }
    public required DbSet<Review> Reviews { get; set; }
    public required DbSet<ReviewReply> ReviewReplies { get; set; }
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Brand> Brands { get; set; }
    public required DbSet<Specification> Specifications { get; set; }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<ProductImage> ProductImages { get; set; }
    public required DbSet<Tag> Tags { get; set; }
    public required DbSet<TagProduct> TagProducts { get; set; }
    public required DbSet<Refund> Refunds { get; set; }
    public required DbSet<RefundStatus> RefundStatuses { get; set; }
    public required DbSet<RefundItem> RefundItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
}
