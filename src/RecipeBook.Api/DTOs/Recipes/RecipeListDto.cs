using RecipeBook.Api.Entities;

namespace RecipeBook.Api.DTOs.Recipes;

/// <summary>Compact recipe used in lists.</summary>
public class RecipeListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;   // Category.Name
    public Difficulty Difficulty { get; set; }
    public int CookingMinutes { get; set; }
    public string AuthorName { get; set; } = string.Empty;     // Author.DisplayName
    public int FavoritesCount { get; set; }                    // Favorites.Count
    public int CommentsCount { get; set; }                     // Comments.Count
}
