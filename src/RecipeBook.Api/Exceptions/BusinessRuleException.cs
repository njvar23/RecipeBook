namespace RecipeBook.Api.Exceptions;

/// <summary>400 — the request is well-formed but breaks a business rule.</summary>
public class BusinessRuleException(string message) : AppException(message, StatusCodes.Status400BadRequest);
