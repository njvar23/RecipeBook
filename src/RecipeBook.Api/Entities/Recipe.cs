namespace RecipeBook.Api.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CookingMinutes { get; set; }
    public int Servings { get; set; }
    public Difficulty Difficulty { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // The user who wrote the recipe
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public ICollection<Step> Steps { get; set; } = new List<Step>();
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}
