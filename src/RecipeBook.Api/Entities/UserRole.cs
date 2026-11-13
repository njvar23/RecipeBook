namespace RecipeBook.Api.Entities;

/// <summary>Roles used in [Authorize(Roles = ...)]. Stored as text in the database.</summary>
public enum UserRole
{
    User,
    Admin
}
