using System.Security.Claims;

namespace Application.Common.Constants;

public static class ClaimsConstants
{
    public const string Email = ClaimTypes.Email;
    public const string Name = ClaimTypes.Name;
    public const string DeviceId = "DeviceId";
    public const string NameIdentifier = "nameid";
    public const string Role = ClaimTypes.Role;
    public const string Permission = "permission";
}
