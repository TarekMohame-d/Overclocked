using System.Security.Claims;
using Application.Common.Constants;

namespace Api.Extensions;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        Claim? userId = context.User.Claims.SingleOrDefault(x => x.Type == ClaimsConstants.NameIdentifier);

        return Guid.TryParse(userId?.Value, out Guid result) ? result : null;
    }

    public static string? GetDeviceId(this HttpContext context)
    {
        Claim? deviceId = context.User.Claims.SingleOrDefault(x => x.Type == ClaimsConstants.DeviceId);

        return deviceId?.Value ?? null;
    }
}
