namespace RecipeBook.Api.Exceptions;

/// <summary>404 — the requested resource does not exist.</summary>
public class NotFoundException(string message) : AppException(message, StatusCodes.Status404NotFound)
{
    public static NotFoundException For(string resource, object id) =>
        new($"{resource} with id {id} was not found.");
}
