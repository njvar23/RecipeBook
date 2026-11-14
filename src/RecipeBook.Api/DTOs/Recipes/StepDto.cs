namespace RecipeBook.Api.DTOs.Recipes;

public class StepDto
{
    public int Order { get; set; }
    public string Instruction { get; set; } = string.Empty;
}
