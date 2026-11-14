using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Validators;

public class CreateRecipeRequestValidator(AppDbContext db)
    : RecipeWriteRequestValidator<CreateRecipeRequest>(db);
