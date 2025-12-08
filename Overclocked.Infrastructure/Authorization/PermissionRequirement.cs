using Microsoft.AspNetCore.Authorization;

namespace Overclocked.Infrastructure.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission ?? throw new ArgumentNullException(nameof(permission));
}
