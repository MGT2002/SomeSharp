using System.Security.Cryptography;
using System.Text;

namespace Habr.BusinessLogic.Helpers;

public class PasswordHelper
{
    public static (string hash, string salt) CreatePasswordHash(string password)
    {
        ArgumentNullException.ThrowIfNull(nameof(password));

        var saltBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        var passwordBytes = Encoding.ASCII.GetBytes(password);
        var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

        string hash = Convert.ToBase64String(SHA256.HashData(combinedBytes));

        string salt = Convert.ToBase64String(saltBytes);

        return (hash, salt);
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);
        var passwordBytes = Encoding.ASCII.GetBytes(password);
        var combinedBytes = saltBytes.Concat(passwordBytes).ToArray();

        var hash = Convert.ToBase64String(SHA256.HashData(combinedBytes));
        return hash.Equals(storedHash);
    }
}
