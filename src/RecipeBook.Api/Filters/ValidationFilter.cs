using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Infrastructure;

namespace RecipeBook.Api.Filters;

/// <summary>
/// Runs the FluentValidation validator of every action argument (body or query object)
/// before the action executes. If anything fails, the action is never called and the client
/// gets 400 with ALL broken rules in the unified error format.
///
/// Why a custom filter instead of FluentValidation.AspNetCore's auto-validation:
/// that package runs validators synchronously, and our validators use MustAsync / CustomAsync
/// (database checks), which would throw AsyncValidatorInvokedSynchronouslyException.
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, List<string>>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                var key = ErrorKeys.ToCamelCasePath(failure.PropertyName);
                if (!errors.TryGetValue(key, out var messages))
                    errors[key] = messages = [];
                messages.Add(failure.ErrorMessage);
            }
        }

        if (errors.Count > 0)
        {
            var response = new ErrorResponse(
                StatusCodes.Status400BadRequest,
                "Validation failed",
                errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));

            context.Result = new BadRequestObjectResult(response);
            return;
        }

        await next();
    }
}
