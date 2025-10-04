using System.Security.Claims;

namespace Application.Common.Constants;

public static class ClaimsConstants
{
    public static string Email = ClaimTypes.Email;
    public static string Name = ClaimTypes.Name;
    public static string DeviceId = "DeviceId";
    public static string NameIdentifier = ClaimTypes.NameIdentifier;
    public static string Role = ClaimTypes.Role;
}
