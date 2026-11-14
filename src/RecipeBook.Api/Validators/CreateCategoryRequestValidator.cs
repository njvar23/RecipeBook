using FluentValidation;
using RecipeBook.Api.DTOs.Categories;

namespace RecipeBook.Api.Validators;

/// <summary>Name uniqueness is checked in CategoryService and returns 409 Conflict.</summary>
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters.");
    }
}
