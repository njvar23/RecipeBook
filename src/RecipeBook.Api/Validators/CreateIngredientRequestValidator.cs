using FluentValidation;
using RecipeBook.Api.DTOs.Ingredients;

namespace RecipeBook.Api.Validators;

/// <summary>Name uniqueness is checked in IngredientService and returns 409 Conflict.</summary>
public class CreateIngredientRequestValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");
    }
}
