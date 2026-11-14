using FluentValidation;
using RecipeBook.Api.DTOs.Common;

namespace RecipeBook.Api.Validators;

/// <summary>Shared ?page / ?pageSize rules for every list endpoint's query object.</summary>
public abstract class PaginationQueryValidator<T> : AbstractValidator<T> where T : PaginationQuery
{
    protected PaginationQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PaginationQuery.MaxPageSize)
            .WithMessage($"Page size must be between 1 and {PaginationQuery.MaxPageSize}.");
    }
}
