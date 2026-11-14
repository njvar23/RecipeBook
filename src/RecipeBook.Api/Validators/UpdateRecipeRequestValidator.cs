using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Validators;

public class UpdateRecipeRequestValidator(AppDbContext db)
    : RecipeWriteRequestValidator<UpdateRecipeRequest>(db);
