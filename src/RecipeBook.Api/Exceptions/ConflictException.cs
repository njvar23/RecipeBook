namespace RecipeBook.Api.Exceptions;

/// <summary>409 — the resource already exists.</summary>
public class ConflictException(string message) : AppException(message, StatusCodes.Status409Conflict);
