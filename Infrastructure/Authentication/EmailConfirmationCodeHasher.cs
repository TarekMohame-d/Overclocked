using System.Security.Cryptography;
using System.Text;
using Application.Abstraction.Services;

namespace Infrastructure.Authentication;

public sealed class EmailConfirmationCodeHasher : IEmailConfirmationCodeHasher
{
    private const int SaltSize = 16;

    public string Hash(string code)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var codeBytes = Encoding.UTF8.GetBytes(code);
        var combinedBytes = new byte[salt.Length + codeBytes.Length];

        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(codeBytes, 0, combinedBytes, salt.Length, codeBytes.Length);

        var hash = SHA256.HashData(combinedBytes);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string code, string codeHash)
    {
        var parts = codeHash.Split('-');

        // Extract the original hash and salt from the stored string
        var hashFromDb = Convert.FromHexString(parts[0]);
        var saltFromDb = Convert.FromHexString(parts[1]);

        // Repeat the exact same hashing process with the provided token and the extracted salt
        var codeBytes = Encoding.UTF8.GetBytes(code);
        var combinedBytes = new byte[saltFromDb.Length + codeBytes.Length];

        Buffer.BlockCopy(saltFromDb, 0, combinedBytes, 0, saltFromDb.Length);
        Buffer.BlockCopy(codeBytes, 0, combinedBytes, saltFromDb.Length, codeBytes.Length);

        var computedHash = SHA256.HashData(combinedBytes);

        // Compare the results in constant time
        return CryptographicOperations.FixedTimeEquals(hashFromDb, computedHash);
    }
}
