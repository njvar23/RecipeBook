using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Auth;

namespace RecipeBook.Api.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(AppDbContext db)
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .MaximumLength(256)
            .MustAsync(async (email, ct) =>
            {
                var normalized = email.Trim().ToLowerInvariant();
                return !await db.Users.AnyAsync(u => u.Email == normalized, ct);
            })
            .WithMessage("A user with this email already exists.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required.")
            .Length(2, 50).WithMessage("Display name must be between 2 and 50 characters.");
    }
}
