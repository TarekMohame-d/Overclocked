using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IRoleRepository : IGenericRepository<Domain.RoleAggregate.Role, RoleId>
{
}
