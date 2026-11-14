using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.Validators;

/// <summary>
/// All the rules for a recipe body. Create and Update share exactly the same rules,
/// so both concrete validators inherit from this one instead of copy-pasting it.
/// </summary>
public abstract class RecipeWriteRequestValidator<T> : AbstractValidator<T> where T : RecipeWriteRequest
{
    private static readonly string AllowedDifficulties = string.Join(", ", Enum.GetNames<Difficulty>());

    protected RecipeWriteRequestValidator(AppDbContext db)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 150).WithMessage("Title must be between 3 and 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be at most 2000 characters.");

        RuleFor(x => x.CookingMinutes)
            .InclusiveBetween(1, 1440).WithMessage("Cooking minutes must be between 1 and 1440.");

        RuleFor(x => x.Servings)
            .InclusiveBetween(1, 50).WithMessage("Servings must be between 1 and 50.");

        RuleFor(x => x.Difficulty)
            .NotEmpty().WithMessage("Difficulty is required.")
            .IsEnumName(typeof(Difficulty), caseSensitive: false)
            .WithMessage($"Difficulty must be one of: {AllowedDifficulties}.");

        RuleFor(x => x.CategoryId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("CategoryId is required.")
            .MustAsync((id, ct) => db.Categories.AnyAsync(c => c.Id == id, ct))
            .WithMessage("Category does not exist.");

        // ---- Steps: the collection as a whole ----
        RuleFor(x => x.Steps)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Recipe must have at least one step.")
            .Must(steps => steps.All(s => s is not null)).WithMessage("Steps cannot contain null items.")
            .Must(steps => steps.Select(s => s.Order).Distinct().Count() == steps.Count)
            .WithMessage("Step orders must be unique.")
            .Must(steps => steps.Select(s => s.Order).Order().SequenceEqual(Enumerable.Range(1, steps.Count)))
            .WithMessage("Step orders must be sequential starting from 1 (1, 2, 3, ...).");

        // ---- Steps: every single item ----
        RuleForEach(x => x.Steps).SetValidator(new StepRequestValidator());

        // ---- Ingredients: the collection as a whole ----
        RuleFor(x => x.Ingredients)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Recipe must have at least one ingredient.")
            .Must(lines => lines.All(l => l is not null)).WithMessage("Ingredients cannot contain null items.")
            .Must(lines => lines.Select(l => l.IngredientId).Distinct().Count() == lines.Count)
            .WithMessage("The same ingredient cannot be listed twice.")
            .CustomAsync(async (lines, context, ct) =>
            {
                // One query for all ids, then report exactly which ones are missing
                var ids = lines.Select(l => l.IngredientId).Where(id => id > 0).Distinct().ToList();
                var existing = await db.Ingredients
                    .Where(i => ids.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync(ct);

                var missing = ids.Except(existing).ToList();
                if (missing.Count > 0)
                    context.AddFailure($"Ingredients do not exist: {string.Join(", ", missing)}.");
            });

        // ---- Ingredients: every single line ----
        RuleForEach(x => x.Ingredients).SetValidator(new IngredientLineValidator());
    }
}
