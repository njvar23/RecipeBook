namespace RecipeBook.Api.DTOs.Auth;

/// <summary>Every registered account gets the "User" role. Admins are created by the seeder.</summary>
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
