using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CartReadRepository(ApplicationDbContext dbContext) : ICartReadRepository
{
    private readonly IQueryable<Cart> _queryable = dbContext.Carts.AsNoTracking();

    public Task<Cart?> GetAsync(UserId userId, CancellationToken ct = default) =>
        _queryable.FirstOrDefaultAsync(x => x.UserId == userId, ct);
}
