using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICartReadRepository : IRepository
{
    Task<Cart?> GetAsync(UserId userId, CancellationToken ct = default);
}
