namespace RecipeBook.Api.DTOs.Auth;

/// <summary>Returned by register (201) — never contains the password hash.</summary>
public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
