using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IWishlistReadRepository : IRepository
{
    Task<Wishlist?> GetAsync(UserId userId, CancellationToken ct = default);
}
