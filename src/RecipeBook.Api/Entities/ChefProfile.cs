namespace RecipeBook.Api.Entities;

public class ChefProfile
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    public string Speciality { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }

    // Dependent side of the 1:1 relationship — unique FK
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
