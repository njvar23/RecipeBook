namespace RecipeBook.Api.DTOs.Recipes;

public class SearchByIngredientsResponse
{
    /// <summary>Recipes whose every ingredient is in the request list.</summary>
    public List<RecipeMatchDto> CanCook { get; set; } = [];

    /// <summary>Recipes missing at most 2 ingredients (listed in MissingIngredients).</summary>
    public List<RecipeMatchDto> AlmostCanCook { get; set; } = [];
}
