namespace RecipeBook.Api.Entities;

/// <summary>
/// Explicit join entity for the Recipe ↔ Ingredient N:M relationship.
/// It must be a real entity (not EF's automatic join table) because it carries
/// extra data: Quantity and Unit. Primary key = (RecipeId, IngredientId).
/// </summary>
public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
}
