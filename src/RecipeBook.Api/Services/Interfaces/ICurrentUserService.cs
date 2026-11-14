namespace RecipeBook.Api.Services.Interfaces;

/// <summary>Who is calling: read from the JWT of the current request.</summary>
public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAdmin { get; }
}
