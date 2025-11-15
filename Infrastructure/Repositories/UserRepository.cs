using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
