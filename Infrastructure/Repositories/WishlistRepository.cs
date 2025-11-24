using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class WishlistRepository(ApplicationDbContext dbContext)
    : GenericRepository<Wishlist>(dbContext), IWishlistRepository
{

}
