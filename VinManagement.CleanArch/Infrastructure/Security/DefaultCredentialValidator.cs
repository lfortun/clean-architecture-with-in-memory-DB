using System.Security.Cryptography;
using System.Text;
using Domain.Services;

namespace Infrastructure.Security;

public class DefaultCredentialValidator : ICredentialValidator
{
    private readonly Dictionary<string, string> _credentials;

    public DefaultCredentialValidator()
    {
        _credentials = new Dictionary<string, string>
        {
            { "admin", HashPassword("admin123") },
            { "user", HashPassword("user123") }
        };
    }

    public Task<bool> IsValidAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(false);
        }

        var isValid = _credentials.TryGetValue(username.ToLowerInvariant(), out var storedHash)
                      && VerifyPassword(password, storedHash);

        return Task.FromResult(isValid);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }
}
