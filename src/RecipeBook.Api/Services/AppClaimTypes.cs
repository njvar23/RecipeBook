namespace RecipeBook.Api.Services;

/// <summary>Short JWT claim names used in our tokens (no Microsoft URI-style claim mapping).</summary>
public static class AppClaimTypes
{
    public const string UserId = "sub";
    public const string Email = "email";
    public const string Role = "role";
    public const string Name = "name";
}
