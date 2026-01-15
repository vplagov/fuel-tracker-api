using System.Security.Cryptography;
using System.Text;

namespace FuelTracker.API.Security;

public class PasswordHasher
{
    private const int KeySize = 64;

    public static string HashPassword(string password)
    {
        const int iterations = 350_000;
        var salt = RandomNumberGenerator.GetBytes(KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations,
            HashAlgorithmName.SHA512, KeySize);
        return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyHashedPassword(string password, string combinedHash)
    {
        var parts = combinedHash.Split(".");
        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hashedPassword = Convert.FromBase64String(parts[2]);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, KeySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, hashedPassword);
    }
}