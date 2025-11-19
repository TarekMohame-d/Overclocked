using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class RolePermissionsRepository : GenericRepository<RolePermission>, IRolePermissionsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RolePermissionsRepository(ApplicationDbContext dbContext)
        : base(dbContext) => _dbContext = dbContext;
}
