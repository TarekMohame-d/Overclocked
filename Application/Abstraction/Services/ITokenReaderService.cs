namespace Application.Abstraction.Services;

public interface ITokenReaderService
{
    IDictionary<string, string>? GetClaimsFromToken(string accessToken);
}
