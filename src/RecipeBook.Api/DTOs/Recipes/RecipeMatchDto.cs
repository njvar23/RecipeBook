namespace RecipeBook.Api.DTOs.Recipes;

public class RecipeMatchDto : RecipeListDto
{
    public int TotalIngredients { get; set; }
    public List<string> MissingIngredients { get; set; } = [];
}
