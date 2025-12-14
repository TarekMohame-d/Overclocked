using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IWishlistRepository : IGenericRepository<Wishlist, WishlistId>
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Wishlist?> GetWishlistAsync(UserId userId, CancellationToken cancellationToken = default);
}
