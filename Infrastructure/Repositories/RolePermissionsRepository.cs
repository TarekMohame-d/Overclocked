using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class RolePermissionsRepository(ApplicationDbContext dbContext)
    : GenericRepository<RolePermission>(dbContext), IRolePermissionsRepository
{

}
