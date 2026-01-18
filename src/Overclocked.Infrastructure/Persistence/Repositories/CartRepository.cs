using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CartRepository(ApplicationDbContext dbContext) : ICartRepository
{
    private readonly DbSet<Cart> _dbSet = dbContext.Carts;

    public Task<Cart?> GetAsync(UserId userId, CancellationToken ct = default) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public Task<bool> ExistsAsync(UserId userId, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.UserId == userId, ct);

    public void Add(Cart cart) => _dbSet.Add(cart);
}
