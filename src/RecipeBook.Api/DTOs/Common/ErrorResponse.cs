namespace RecipeBook.Api.DTOs.Common;

/// <summary>
/// The single error format used for every 4xx response:
/// { "status": 400, "message": "Validation failed", "errors": { "title": ["..."] } }
/// </summary>
public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }

    public ErrorResponse() { }

    public ErrorResponse(int status, string message, IDictionary<string, string[]>? errors = null)
    {
        Status = status;
        Message = message;
        Errors = errors;
    }
}
