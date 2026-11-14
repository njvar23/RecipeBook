using Microsoft.AspNetCore.Diagnostics;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Exceptions;

namespace RecipeBook.Api.Infrastructure;

/// <summary>
/// Translates exceptions thrown anywhere in the pipeline into the unified error response.
/// Services throw NotFound / BusinessRule / Forbidden / Conflict exceptions,
/// and this one place maps them to 404 / 400 / 403 / 409.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        ErrorResponse response;

        if (exception is AppException appException)
        {
            response = new ErrorResponse(appException.StatusCode, appException.Message);
        }
        else
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);
            response = new ErrorResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }

        httpContext.Response.StatusCode = response.Status;
        await httpContext.Response.WriteAsJsonAsync(response, ct);
        return true;
    }
}
