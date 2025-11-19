using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartRepository(ApplicationDbContext dbContext) : GenericRepository<Cart>(dbContext), ICartRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Cart?> GetCartWithItemsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Carts.AsTracking()
            .Include(x => x.CartItems)
            .SingleOrDefaultAsync(x => x.UserId == userId, cancellationToken: cancellationToken);
    }
}
