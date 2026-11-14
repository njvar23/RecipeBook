namespace RecipeBook.Api.DTOs.Recipes;

public class RecipeIngredientRequest
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gram / Ml / Piece / Tbsp / Tsp / Cup. Kept as a string on purpose: a wrong value
    /// then reaches FluentValidation and is reported together with every other error,
    /// instead of failing JSON deserialization of the whole body.
    /// </summary>
    public string Unit { get; set; } = string.Empty;
}
