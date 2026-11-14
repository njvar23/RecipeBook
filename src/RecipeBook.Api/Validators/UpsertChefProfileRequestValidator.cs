using FluentValidation;
using RecipeBook.Api.DTOs.ChefProfiles;

namespace RecipeBook.Api.Validators;

public class UpsertChefProfileRequestValidator : AbstractValidator<UpsertChefProfileRequest>
{
    public UpsertChefProfileRequestValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio must be at most 1000 characters.");

        RuleFor(x => x.Speciality)
            .NotEmpty().WithMessage("Speciality is required.")
            .Length(2, 80).WithMessage("Speciality must be between 2 and 80 characters.");

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 70).WithMessage("Years of experience must be between 0 and 70.");
    }
}
