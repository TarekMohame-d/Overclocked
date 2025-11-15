using Microsoft.AspNetCore.Authorization;

namespace Api.ActionFilters;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission ?? throw new ArgumentNullException(nameof(permission));
}
