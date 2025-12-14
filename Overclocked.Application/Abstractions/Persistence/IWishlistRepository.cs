using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate.ValueObjects;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IWishlistRepository : IGenericRepository<WishlistEntity, WishlistId>
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<WishlistEntity?> GetAsync(UserId userId, CancellationToken cancellationToken = default);
}
