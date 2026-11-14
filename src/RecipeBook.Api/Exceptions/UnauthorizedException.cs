namespace RecipeBook.Api.Exceptions;

/// <summary>401 — wrong credentials (used by login).</summary>
public class UnauthorizedException(string message) : AppException(message, StatusCodes.Status401Unauthorized);
