using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IWishlistRepository : IRepository
{
    Task<Wishlist?> GetAsync(UserId userId, CancellationToken ct = default);

    Task<bool> ExistsAsync(UserId userId, CancellationToken ct = default);

    void Add(Wishlist wishlist);
}
