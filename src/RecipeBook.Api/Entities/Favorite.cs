namespace RecipeBook.Api.Entities;

/// <summary>
/// Join entity for the User ↔ Recipe "favorites" N:M relationship.
/// Composite primary key (UserId, RecipeId) — a user can favorite a recipe only once.
/// </summary>
public class Favorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public DateTime AddedAt { get; set; }
}
