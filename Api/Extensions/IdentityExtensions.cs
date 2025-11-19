using System.Security.Claims;
using Application.Common.Constants;

namespace Api.Extensions;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userId, out Guid result) ? result : null;
    }

    public static string? GetDeviceId(this HttpContext context)
    {
        Claim? deviceId = context.User.Claims.SingleOrDefault(x => x.Type == ClaimsConstants.DeviceId);

        return deviceId?.Value ?? null;
    }
}
