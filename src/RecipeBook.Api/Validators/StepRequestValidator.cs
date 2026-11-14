using FluentValidation;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Validators;

/// <summary>Validates one step. Attached to the recipe validators with RuleForEach + SetValidator.</summary>
public class StepRequestValidator : AbstractValidator<StepRequest>
{
    public StepRequestValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0).WithMessage("Step order must be a positive number.");

        RuleFor(x => x.Instruction)
            .NotEmpty().WithMessage("Instruction is required.")
            .Length(3, 500).WithMessage("Instruction must be between 3 and 500 characters.");
    }
}
