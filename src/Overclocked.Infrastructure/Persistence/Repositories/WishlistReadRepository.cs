using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class WishlistReadRepository(ApplicationDbContext dbContext) : IWishlistReadRepository
{
    private readonly IQueryable<Wishlist> _queryable = dbContext.Wishlists.AsNoTracking();

    public Task<Wishlist?> GetAsync(UserId userId, CancellationToken ct = default) =>
        _queryable.FirstOrDefaultAsync(x => x.UserId == userId, ct);
}
