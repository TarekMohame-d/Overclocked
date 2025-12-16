using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Infrastructure.Persistence.Entities;

public class PermissionLookup
{
    public Permission Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
