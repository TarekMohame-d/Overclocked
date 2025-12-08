using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CartRepository(ApplicationDbContext context)
    : GenericRepository<Cart, CartId>(context), ICartRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<Cart?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _context.Carts.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
