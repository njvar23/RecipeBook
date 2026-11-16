using System.Net;
using System.Net.Http.Json;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;
using RecipeBook.Tests.Infrastructure;

namespace RecipeBook.Tests;

public class RecipeTests(RecipeBookApiFactory factory) : IClassFixture<RecipeBookApiFactory>
{
    private static CreateRecipeRequest ValidRecipe(string title = "ტესტის რეცეპტი") => new()
    {
        Title = title,
        Description = "აღწერა",
        CookingMinutes = 30,
        Servings = 2,
        Difficulty = "Easy",
        CategoryId = 2,
        Steps =
        [
            new StepRequest { Order = 2, Instruction = "მეორე ნაბიჯი" },
            new StepRequest { Order = 1, Instruction = "პირველი ნაბიჯი" }
        ],
        Ingredients =
        [
            new RecipeIngredientRequest { IngredientId = 12, Quantity = 2, Unit = "Piece" },
            new RecipeIngredientRequest { IngredientId = 5, Quantity = 0.5m, Unit = "Tsp" }
        ]
    };

    [Fact]
    public async Task Create_recipe_with_nested_steps_and_ingredients_returns_201_and_full_details()
    {
        var (client, userId) = await factory.NewUserAsync("შეფი");

        var response = await client.PostAsJsonAsync("/api/recipes", ValidRecipe());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        var recipe = await response.ReadAsync<RecipeDetailsDto>();
        Assert.Equal(userId, recipe.AuthorId);
        Assert.Equal("შეფი", recipe.AuthorName);
        Assert.Null(recipe.AuthorSpeciality);                  // no chef profile yet
        Assert.Equal("სალათები", recipe.CategoryName);
        Assert.Equal([1, 2], recipe.Steps.Select(s => s.Order)); // sorted by Order
        Assert.Contains(recipe.Ingredients, i => i.IngredientName == "პომიდორი" && i.Unit == MeasurementUnit.Piece);
        Assert.Null(recipe.UpdatedAt);
    }

    [Fact]
    public async Task Create_recipe_with_bad_body_returns_all_validation_errors()
    {
        var (client, _) = await factory.NewUserAsync();
        var bad = new CreateRecipeRequest
        {
            Title = "ab",
            CookingMinutes = 0,
            Servings = 51,
            Difficulty = "Super",
            CategoryId = 999,
            Steps = [new StepRequest { Order = 1, Instruction = "ok step" }, new StepRequest { Order = 3, Instruction = "x" }],
            Ingredients =
            [
                new RecipeIngredientRequest { IngredientId = 1, Quantity = 0, Unit = "Kg" },
                new RecipeIngredientRequest { IngredientId = 9999, Quantity = 1, Unit = "Gram" }
            ]
        };

        var response = await client.PostAsJsonAsync("/api/recipes", bad);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await response.ReadAsync<ErrorResponse>()).Errors!;
        foreach (var key in new[] { "title", "cookingMinutes", "servings", "difficulty", "categoryId", "steps",
                     "steps[1].instruction", "ingredients", "ingredients[0].quantity", "ingredients[0].unit" })
        {
            Assert.True(errors.ContainsKey(key), $"Expected an error for '{key}'");
        }
        Assert.Contains("Step orders must be sequential starting from 1 (1, 2, 3, ...).", errors["steps"]);
        Assert.Contains("Ingredients do not exist: 9999.", errors["ingredients"]);
    }

    [Fact]
    public async Task Duplicate_step_orders_and_duplicate_ingredients_are_rejected()
    {
        var (client, _) = await factory.NewUserAsync();
        var request = ValidRecipe();
        request.Steps = [new() { Order = 1, Instruction = "one" }, new() { Order = 1, Instruction = "one again" }];
        request.Ingredients = [new() { IngredientId = 1, Quantity = 1, Unit = "Cup" }, new() { IngredientId = 1, Quantity = 2, Unit = "Cup" }];

        var response = await client.PostAsJsonAsync("/api/recipes", request);

        var errors = (await response.ReadAsync<ErrorResponse>()).Errors!;
        Assert.Contains("Step orders must be unique.", errors["steps"]);
        Assert.Contains("The same ingredient cannot be listed twice.", errors["ingredients"]);
    }

    [Fact]
    public async Task Update_replaces_steps_and_ingredients_and_sets_UpdatedAt()
    {
        var (client, _) = await factory.NewUserAsync();
        var created = await (await client.PostAsJsonAsync("/api/recipes", ValidRecipe())).ReadAsync<RecipeDetailsDto>();

        var update = new UpdateRecipeRequest
        {
            Title = "განახლებული",
            CookingMinutes = 15,
            Servings = 1,
            Difficulty = "Hard",
            CategoryId = 1,
            Steps = [new() { Order = 1, Instruction = "ერთადერთი ნაბიჯი" }],
            Ingredients = [new() { IngredientId = 14, Quantity = 100, Unit = "Gram" }]
        };
        var response = await client.PutAsJsonAsync($"/api/recipes/{created.Id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.ReadAsync<RecipeDetailsDto>();
        Assert.Equal("განახლებული", updated.Title);
        Assert.Equal(Difficulty.Hard, updated.Difficulty);
        Assert.Single(updated.Steps);
        Assert.Equal("ნიგოზი", Assert.Single(updated.Ingredients).IngredientName);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task Only_author_or_admin_can_update_or_delete()
    {
        var (author, _) = await factory.NewUserAsync();
        var (stranger, _) = await factory.NewUserAsync();
        var admin = await factory.LoginAsync(DbSeeder.AdminEmail, DbSeeder.AdminPassword);
        var recipe = await (await author.PostAsJsonAsync("/api/recipes", ValidRecipe())).ReadAsync<RecipeDetailsDto>();

        var strangerUpdate = await stranger.PutAsJsonAsync($"/api/recipes/{recipe.Id}", ValidRecipe("სხვისი"));
        var strangerDelete = await stranger.DeleteAsync($"/api/recipes/{recipe.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, strangerUpdate.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, strangerDelete.StatusCode);

        var adminDelete = await admin.DeleteAsync($"/api/recipes/{recipe.Id}");
        Assert.Equal(HttpStatusCode.NoContent, adminDelete.StatusCode);

        var afterDelete = await author.GetAsync($"/api/recipes/{recipe.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Deleting_a_recipe_also_deletes_its_comments_and_favorites()
    {
        var (author, _) = await factory.NewUserAsync();
        var (fan, _) = await factory.NewUserAsync();
        var recipe = await (await author.PostAsJsonAsync("/api/recipes", ValidRecipe())).ReadAsync<RecipeDetailsDto>();
        await fan.PostAsJsonAsync($"/api/recipes/{recipe.Id}/comments", new { text = "მომწონს" });
        await fan.PostAsync($"/api/recipes/{recipe.Id}/favorite", null);

        var delete = await author.DeleteAsync($"/api/recipes/{recipe.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var favorites = await (await fan.GetAsync("/api/me/favorites")).ReadAsync<PagedResult<object>>();
        Assert.Equal(0, favorites.TotalCount);
    }

    [Fact]
    public async Task List_supports_filters_and_pagination()
    {
        var client = await factory.LoginAsync(DbSeeder.NinoEmail, DbSeeder.NinoPassword);

        var page = await (await client.GetAsync("/api/recipes?pageSize=2&page=1")).ReadAsync<PagedResult<RecipeListDto>>();
        Assert.Equal(2, page.Items.Count);
        Assert.True(page.TotalCount >= 4);
        Assert.Equal(2, page.PageSize);

        var soups = await (await client.GetAsync("/api/recipes?categoryId=1")).ReadAsync<PagedResult<RecipeListDto>>();
        Assert.All(soups.Items, r => Assert.Equal("სუპები", r.CategoryName));

        var walnut = await (await client.GetAsync("/api/recipes?ingredientId=14&difficulty=Easy&maxMinutes=40"))
            .ReadAsync<PagedResult<RecipeListDto>>();
        Assert.Contains(walnut.Items, r => r.Title == "ბადრიჯანი ნიგვზით");
        Assert.All(walnut.Items, r => Assert.True(r.CookingMinutes <= 40));

        var badPaging = await client.GetAsync("/api/recipes?pageSize=0");
        Assert.Equal(HttpStatusCode.BadRequest, badPaging.StatusCode);
    }

    [Fact]
    public async Task Details_show_favorite_flag_counts_and_author_speciality()
    {
        var giorgi = await factory.LoginAsync(DbSeeder.GiorgiEmail, DbSeeder.GiorgiPassword);
        var list = await (await giorgi.GetAsync("/api/recipes?search=აჭარული")).ReadAsync<PagedResult<RecipeListDto>>();
        var khachapuriId = Assert.Single(list.Items).Id;

        var details = await (await giorgi.GetAsync($"/api/recipes/{khachapuriId}")).ReadAsync<RecipeDetailsDto>();

        Assert.True(details.IsFavorite);                       // seeded favorite of Giorgi
        Assert.Equal("ქართული ცომეული", details.AuthorSpeciality); // Nino's chef profile
        Assert.True(details.CommentsCount >= 1);
        Assert.Equal(details.Comments.Count, details.CommentsCount);
    }

    [Fact]
    public async Task Search_by_ingredients_returns_full_and_partial_matches()
    {
        var client = await factory.LoginAsync(DbSeeder.NinoEmail, DbSeeder.NinoPassword);

        // Everything the seeded salad needs: tomato, cucumber, onion, walnut, garlic, salt
        var full = await (await client.PostAsJsonAsync("/api/recipes/search-by-ingredients",
            new SearchByIngredientsRequest { IngredientIds = [12, 13, 10, 14, 11, 5] })).ReadAsync<SearchByIngredientsResponse>();
        Assert.Contains(full.CanCook, r => r.Title.StartsWith("პომიდვრისა"));

        // Without garlic and salt the salad lacks exactly 2 ingredients
        var partial = await (await client.PostAsJsonAsync("/api/recipes/search-by-ingredients",
            new SearchByIngredientsRequest { IngredientIds = [12, 13, 10, 14] })).ReadAsync<SearchByIngredientsResponse>();
        var salad = Assert.Single(partial.AlmostCanCook, r => r.Title.StartsWith("პომიდვრისა"));
        Assert.Equal(["მარილი", "ნიორი"], salad.MissingIngredients);
        Assert.DoesNotContain(partial.CanCook, r => r.Title.StartsWith("პომიდვრისა"));
    }
}
