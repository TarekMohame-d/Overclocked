using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class WishlistRepository(ApplicationDbContext dbContext) : IWishlistRepository
{
    private readonly DbSet<Wishlist> _dbSet = dbContext.Wishlists;

    public Task<Wishlist?> GetAsync(UserId userId, CancellationToken ct = default) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public Task<bool> ExistsAsync(UserId userId, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.UserId == userId, ct);

    public void Add(Wishlist wishlist) => _dbSet.Add(wishlist);
}
