using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context)
    : GenericRepository<Role, RoleId>(context), IRoleRepository
{
}
