namespace RecipeBook.Api.Exceptions;

/// <summary>403 — the user is authenticated but is not allowed to touch this resource (ownership).</summary>
public class ForbiddenException(string message = "You do not have permission to perform this action.")
    : AppException(message, StatusCodes.Status403Forbidden);
