using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(ApplicationDbContext dbContext)
    : GenericRepository<RefreshToken>(dbContext), IRefreshTokenRepository
{

}
