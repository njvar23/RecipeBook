namespace RecipeBook.Api.DTOs.Recipes;

public class StepRequest
{
    public int Order { get; set; }
    public string Instruction { get; set; } = string.Empty;
}
