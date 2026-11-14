using FluentValidation;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Validators;

/// <summary>Validates one ingredient line of a recipe. Attached with RuleForEach + SetValidator.</summary>
public class IngredientLineValidator : AbstractValidator<RecipeIngredientRequest>
{
    private static readonly string AllowedUnits = string.Join(", ", Enum.GetNames<MeasurementUnit>());

    public IngredientLineValidator()
    {
        RuleFor(x => x.IngredientId)
            .GreaterThan(0).WithMessage("IngredientId must be a positive number.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Quantity must be at most 10000.");

        RuleFor(x => x.Unit)
            .NotEmpty().WithMessage("Unit is required.")
            .IsEnumName(typeof(MeasurementUnit), caseSensitive: false)
            .WithMessage($"Unit must be one of: {AllowedUnits}.");
    }
}
