using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class WishlistRepository(ApplicationDbContext context)
    : GenericRepository<Wishlist, WishlistId>(context), IWishlistRepository
{
    public Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Wishlists.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<Wishlist?> GetAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Wishlists.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
