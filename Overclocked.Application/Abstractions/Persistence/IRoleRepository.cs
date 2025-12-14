using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IRoleRepository : IGenericRepository<Domain.RoleAggregate.Role, RoleId>
{
}
