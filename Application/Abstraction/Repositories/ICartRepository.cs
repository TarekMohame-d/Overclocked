using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartWithItemsAsync(Guid userId, CancellationToken cancellationToken);
}
