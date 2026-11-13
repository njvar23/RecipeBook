namespace RecipeBook.Api.Entities;

public class Step
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int Order { get; set; }
    public string Instruction { get; set; } = string.Empty;
}
