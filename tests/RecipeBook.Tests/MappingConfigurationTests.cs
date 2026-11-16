using AutoMapper;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;

namespace RecipeBook.Tests;

public class MappingConfigurationTests
{
    private static readonly MapperConfiguration Config =
        new(cfg => cfg.AddMaps(typeof(Program).Assembly));

    [Fact]
    public void AutoMapper_configuration_is_valid()
    {
        // Fails if any DTO member is left without a mapping
        Config.AssertConfigurationIsValid();
    }

    [Fact]
    public void CreateRecipeRequest_maps_to_recipe_with_nested_steps_and_ingredients()
    {
        var mapper = Config.CreateMapper();
        var request = new CreateRecipeRequest
        {
            Title = "  ხაჭაპური  ",
            CookingMinutes = 45,
            Servings = 2,
            Difficulty = "medium",
            CategoryId = 3,
            Steps =
            [
                new StepRequest { Order = 1, Instruction = "მოზილეთ ცომი" },
                new StepRequest { Order = 2, Instruction = "გამოაცხვეთ" }
            ],
            Ingredients =
            [
                new RecipeIngredientRequest { IngredientId = 1, Quantity = 500, Unit = "Gram" },
                new RecipeIngredientRequest { IngredientId = 7, Quantity = 2, Unit = "piece" }
            ]
        };

        var recipe = mapper.Map<Recipe>(request);

        Assert.Equal("ხაჭაპური", recipe.Title);
        Assert.Equal(Difficulty.Medium, recipe.Difficulty);
        Assert.Equal(2, recipe.Steps.Count);
        Assert.Equal(2, recipe.RecipeIngredients.Count);
        Assert.Contains(recipe.RecipeIngredients, ri => ri.IngredientId == 7 && ri.Unit == MeasurementUnit.Piece);
    }

    [Fact]
    public void Recipe_maps_to_details_with_sorted_steps_and_flat_ingredients()
    {
        var mapper = Config.CreateMapper();
        var recipe = new Recipe
        {
            Id = 1,
            Title = "Test",
            Category = new Category { Name = "სუპები" },
            Author = new User { DisplayName = "ნინო", ChefProfile = new ChefProfile { Speciality = "სუპები" } },
            Steps = [new Step { Order = 2, Instruction = "second" }, new Step { Order = 1, Instruction = "first" }],
            RecipeIngredients = [new RecipeIngredient { Ingredient = new Ingredient { Name = "ხახვი" }, Quantity = 1, Unit = MeasurementUnit.Piece }]
        };

        var dto = mapper.Map<RecipeDetailsDto>(recipe);

        Assert.Equal("სუპები", dto.CategoryName);
        Assert.Equal("ნინო", dto.AuthorName);
        Assert.Equal("სუპები", dto.AuthorSpeciality);
        Assert.Equal([1, 2], dto.Steps.Select(s => s.Order));
        Assert.Equal("ხახვი", dto.Ingredients.Single().IngredientName);
    }
}
