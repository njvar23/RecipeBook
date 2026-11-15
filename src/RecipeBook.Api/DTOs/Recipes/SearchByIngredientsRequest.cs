namespace RecipeBook.Api.DTOs.Recipes;

/// <summary>Bonus: "what can I cook?" — the ingredients I have at home.</summary>
public class SearchByIngredientsRequest
{
    public List<int> IngredientIds { get; set; } = [];
}
