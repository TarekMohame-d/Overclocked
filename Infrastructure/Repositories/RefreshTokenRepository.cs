using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext) => _dbContext = dbContext;
}
