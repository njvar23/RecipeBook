using RecipeBook.Api.Entities;

namespace RecipeBook.Api.DTOs.Favorites;

/// <summary>A favorited recipe, flattened from Favorite → Recipe.</summary>
public class FavoriteDto
{
    public int RecipeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public int CookingMinutes { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
