namespace RecipeBook.Api.DTOs.Recipes;

/// <summary>
/// Shared shape of CreateRecipeRequest and UpdateRecipeRequest (both carry the full recipe
/// with its nested steps and ingredients). Each concrete request still has its own validator.
/// </summary>
public abstract class RecipeWriteRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CookingMinutes { get; set; }
    public int Servings { get; set; }

    /// <summary>Easy / Medium / Hard (string for the same reason as RecipeIngredientRequest.Unit).</summary>
    public string Difficulty { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public List<StepRequest> Steps { get; set; } = [];
    public List<RecipeIngredientRequest> Ingredients { get; set; } = [];
}
