using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CartRepository(ApplicationDbContext context)
    : GenericRepository<Cart, CartId>(context), ICartRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Carts.AnyAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<Cart?> GetAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Carts.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
