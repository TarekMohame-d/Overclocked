using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Infrastructure.Persistence.Entities;

public class RoleLookup
{
    public Role Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
