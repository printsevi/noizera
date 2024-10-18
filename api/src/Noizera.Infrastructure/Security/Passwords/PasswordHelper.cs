using Noizera.Shared.Domain.Users;
using System.Security.Cryptography;
using System.Text;

namespace Noizera.Infrastructure.Security.Passwords;

public class PasswordHelper : IPasswordHelper
{
    private const int keySize = 64;
    private const int iterations = 350000;
    private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

    public string HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(keySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);

        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash, IEnumerable<byte> salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt.ToArray(), iterations, hashAlgorithm, keySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
