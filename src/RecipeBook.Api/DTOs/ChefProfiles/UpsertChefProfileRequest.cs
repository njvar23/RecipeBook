namespace RecipeBook.Api.DTOs.ChefProfiles;

public class UpsertChefProfileRequest
{
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
}
