namespace RecipeBook.Api.DTOs.ChefProfiles;

public class ChefProfileDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;   // from User.DisplayName
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public int RecipesCount { get; set; }                     // computed: User.Recipes.Count
}
