namespace Noizera.Common.Domain.Users;

public interface IPasswordHelper
{
    string HashPassword(string password, out byte[] salt);

    bool VerifyPassword(string password, string hash, IEnumerable<byte> salt);
}