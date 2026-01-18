using System.Security.Cryptography;
using Overclocked.Application.Abstractions.Services;

namespace Overclocked.Infrastructure.Authentication;

public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 500000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(value, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string value, string hash)
    {
        var parts = hash.Split('-');
        var hashPart = Convert.FromHexString(parts[0]);
        var salt = Convert.FromHexString(parts[1]);

        var inputHash = Rfc2898DeriveBytes.Pbkdf2(value, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(hashPart, inputHash);
    }
}
