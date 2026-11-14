using FluentValidation;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Validators;

public class RecipeQueryParametersValidator : PaginationQueryValidator<RecipeQueryParameters>
{
    public RecipeQueryParametersValidator()
    {
        RuleFor(x => x.MaxMinutes)
            .GreaterThan(0).When(x => x.MaxMinutes.HasValue)
            .WithMessage("maxMinutes must be greater than 0.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().When(x => x.Difficulty.HasValue)
            .WithMessage("Difficulty must be Easy, Medium or Hard.");

        RuleFor(x => x.Search)
            .MaximumLength(100).WithMessage("Search text must be at most 100 characters.");
    }
}
