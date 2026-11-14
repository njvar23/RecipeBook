using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

/// <summary>Only the BCrypt hash (with its random salt) is ever stored — never the plain password.</summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
