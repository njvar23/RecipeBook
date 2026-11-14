using RecipeBook.Api.DTOs.Comments;

namespace RecipeBook.Api.DTOs.Recipes;

/// <summary>Full recipe: everything from the list DTO plus steps, ingredients and comments.</summary>
public class RecipeDetailsDto : RecipeListDto
{
    public string? Description { get; set; }
    public int Servings { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }
    public string? AuthorSpeciality { get; set; }   // null when the author has no chef profile
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<StepDto> Steps { get; set; } = [];                    // sorted by Order
    public List<RecipeIngredientDto> Ingredients { get; set; } = [];
    public List<CommentDto> Comments { get; set; } = [];
    public bool IsFavorite { get; set; }            // for the current user; filled in by the service
}
