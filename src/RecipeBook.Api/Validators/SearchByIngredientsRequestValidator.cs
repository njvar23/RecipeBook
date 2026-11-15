using FluentValidation;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Validators;

public class SearchByIngredientsRequestValidator : AbstractValidator<SearchByIngredientsRequest>
{
    public SearchByIngredientsRequestValidator()
    {
        RuleFor(x => x.IngredientIds)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Provide at least one ingredient id.")
            .Must(ids => ids.Count <= 100).WithMessage("At most 100 ingredient ids are allowed.")
            .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("Ingredient ids must be unique.");

        RuleForEach(x => x.IngredientIds)
            .GreaterThan(0).WithMessage("Ingredient id must be a positive number.");
    }
}
