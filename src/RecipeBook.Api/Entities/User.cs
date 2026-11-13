namespace RecipeBook.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }

    // 1:1 (optional) — a user may or may not have a chef profile
    public ChefProfile? ChefProfile { get; set; }

    // 1:N — recipes written by this user
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    // 1:N — comments written by this user
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // N:M with Recipe through the Favorite join entity
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
