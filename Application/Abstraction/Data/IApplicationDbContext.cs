using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstraction.Data;

public interface IApplicationDbContext
{
    public DbSet<Permission> Permissions { get; }
    public DbSet<RolePermission> RolePermissions { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<EmployeeActivityLog> EmployeeActivityLogs { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }
    public DbSet<EmailConfirmationCode> EmailConfirmationCodes { get; }
    public DbSet<User> Users { get; }
    public DbSet<Address> Addresses { get; }
    public DbSet<Shipment> Shipments { get; }
    public DbSet<ShipmentStatus> ShipmentStatuses { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<OrderItem> OrderItems { get; }
    public DbSet<OrderStatus> OrderStatuses { get; }
    public DbSet<Payment> Payments { get; }
    public DbSet<PaymentMethod> PaymentMethods { get; }
    public DbSet<PaymentStatus> PaymentStatuses { get; }
    public DbSet<Cart> Carts { get; }
    public DbSet<CartItem> CartItems { get; }
    public DbSet<Wishlist> Wishlists { get; }
    public DbSet<WishlistItem> WishlistItems { get; }
    public DbSet<Review> Reviews { get; }
    public DbSet<ReviewReply> ReviewReplies { get; }
    public DbSet<Product> Products { get; }
    public DbSet<Brand> Brands { get; }
    public DbSet<Specification> Specifications { get; }
    public DbSet<Category> Categories { get; }
    public DbSet<ProductImage> ProductImages { get; }
    public DbSet<Tag> Tags { get; }
    public DbSet<TagProduct> TagProducts { get; }
    public DbSet<Refund> Refunds { get; }
    public DbSet<RefundStatus> RefundStatuses { get; }
    public DbSet<RefundItem> RefundItems { get; }
}
