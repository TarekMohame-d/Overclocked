using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class WishlistRepository(ApplicationDbContext context)
    : GenericRepository<Wishlist, WishlistId>(context), IWishlistRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Wishlists.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<Wishlist?> GetWishlistAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Wishlists.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
