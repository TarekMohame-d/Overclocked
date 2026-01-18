using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICartRepository : IRepository
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken ct = default);

    Task<Cart?> GetAsync(UserId userId, CancellationToken ct = default);

    void Add(Cart cart);
}
