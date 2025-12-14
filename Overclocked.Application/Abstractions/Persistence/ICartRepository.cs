using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICartRepository : IGenericRepository<CartEntity, CartId>
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<CartEntity?> GetCartAsync(UserId userId, CancellationToken cancellationToken = default);
}
