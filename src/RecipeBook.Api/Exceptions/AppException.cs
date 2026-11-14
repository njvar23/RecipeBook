namespace RecipeBook.Api.Exceptions;

/// <summary>
/// Base class for expected errors thrown by services.
/// The global exception handler turns them into the unified error response
/// with the matching HTTP status code, so controllers need no try/catch.
/// </summary>
public abstract class AppException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
