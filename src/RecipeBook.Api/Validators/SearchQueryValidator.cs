using FluentValidation;
using RecipeBook.Api.DTOs.Common;

namespace RecipeBook.Api.Validators;

public class SearchQueryValidator : PaginationQueryValidator<SearchQuery>
{
    public SearchQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(100).WithMessage("Search text must be at most 100 characters.");
    }
}
