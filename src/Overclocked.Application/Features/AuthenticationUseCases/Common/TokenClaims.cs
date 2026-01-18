namespace Overclocked.Application.Features.AuthenticationUseCases.Common;

public record TokenClaims(string UserId, string Email, string DeviceId, string Role, IEnumerable<string> Permissions);
