using Microsoft.EntityFrameworkCore;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.EmployeeActivityLogAggregate;
using Overclocked.Domain.PermissionAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Infrastructure.Outbox;

namespace Overclocked.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public required DbSet<Permission> Permissions { get; set; }
    public required DbSet<Role> Roles { get; set; }
    public required DbSet<EmployeeActivityLog> EmployeeActivityLogs { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<Cart> Carts { get; set; }
    public required DbSet<Wishlist> Wishlists { get; set; }
    public required DbSet<Review> Reviews { get; set; }
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Brand> Brands { get; set; }
    public required DbSet<Category> Categories { get; set; }
    public required DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
