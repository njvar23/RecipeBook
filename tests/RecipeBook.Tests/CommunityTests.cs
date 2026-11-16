using System.Net;
using System.Net.Http.Json;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.DTOs.ChefProfiles;
using RecipeBook.Api.DTOs.Comments;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Favorites;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Tests.Infrastructure;

namespace RecipeBook.Tests;

/// <summary>Chef profiles, comments, favorites and admin lookups.</summary>
public class CommunityTests(RecipeBookApiFactory factory) : IClassFixture<RecipeBookApiFactory>
{
    private async Task<int> CreateRecipeAsync(HttpClient author)
    {
        var request = new CreateRecipeRequest
        {
            Title = "საცდელი კერძი",
            CookingMinutes = 20,
            Servings = 2,
            Difficulty = "Medium",
            CategoryId = 4,
            Steps = [new() { Order = 1, Instruction = "მოამზადეთ" }],
            Ingredients = [new() { IngredientId = 18, Quantity = 500, Unit = "Gram" }]
        };
        var response = await author.PostAsJsonAsync("/api/recipes", request);
        response.EnsureSuccessStatusCode();
        return (await response.ReadAsync<RecipeDetailsDto>()).Id;
    }

    [Fact]
    public async Task Chef_profile_upsert_creates_then_updates()
    {
        var (client, userId) = await factory.NewUserAsync("მზარეული");

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/me/chef-profile")).StatusCode);

        var create = await client.PutAsJsonAsync("/api/me/chef-profile",
            new UpsertChefProfileRequest { Bio = "ბიო", Speciality = "სუპები", YearsOfExperience = 3 });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var update = await client.PutAsJsonAsync("/api/me/chef-profile",
            new UpsertChefProfileRequest { Speciality = "დესერტები", YearsOfExperience = 4 });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        await CreateRecipeAsync(client);
        var other = await factory.LoginAsync(DbSeeder.GiorgiEmail, DbSeeder.GiorgiPassword);
        var profile = await (await other.GetAsync($"/api/users/{userId}/chef-profile")).ReadAsync<ChefProfileDto>();
        Assert.Equal("დესერტები", profile.Speciality);
        Assert.Equal("მზარეული", profile.DisplayName);
        Assert.Equal(1, profile.RecipesCount);
    }

    [Fact]
    public async Task Chef_profile_validation_errors()
    {
        var (client, _) = await factory.NewUserAsync();

        var response = await client.PutAsJsonAsync("/api/me/chef-profile",
            new UpsertChefProfileRequest { Bio = new string('a', 1001), Speciality = "x", YearsOfExperience = 71 });

        var errors = (await response.ReadAsync<ErrorResponse>()).Errors!;
        Assert.Equal(["bio", "speciality", "yearsOfExperience"], errors.Keys.Order());
    }

    [Fact]
    public async Task Favorites_add_conflict_list_and_remove()
    {
        var (author, _) = await factory.NewUserAsync();
        var (fan, _) = await factory.NewUserAsync();
        var recipeId = await CreateRecipeAsync(author);

        Assert.Equal(HttpStatusCode.Created, (await fan.PostAsync($"/api/recipes/{recipeId}/favorite", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await fan.PostAsync($"/api/recipes/{recipeId}/favorite", null)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await author.PostAsync($"/api/recipes/{recipeId}/favorite", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await fan.PostAsync("/api/recipes/99999/favorite", null)).StatusCode);

        var mine = await (await fan.GetAsync("/api/me/favorites")).ReadAsync<PagedResult<FavoriteDto>>();
        Assert.Equal("საცდელი კერძი", Assert.Single(mine.Items).Title);

        Assert.Equal(HttpStatusCode.NoContent, (await fan.DeleteAsync($"/api/recipes/{recipeId}/favorite")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await fan.DeleteAsync($"/api/recipes/{recipeId}/favorite")).StatusCode);
    }

    [Fact]
    public async Task Comment_can_be_deleted_by_its_author_recipe_author_or_admin_only()
    {
        var (recipeAuthor, _) = await factory.NewUserAsync();
        var (commenter, _) = await factory.NewUserAsync();
        var (stranger, _) = await factory.NewUserAsync();
        var admin = await factory.LoginAsync(DbSeeder.AdminEmail, DbSeeder.AdminPassword);
        var recipeId = await CreateRecipeAsync(recipeAuthor);

        async Task<int> CommentAsync()
        {
            var r = await commenter.PostAsJsonAsync($"/api/recipes/{recipeId}/comments", new CreateCommentRequest { Text = "გემრიელია" });
            Assert.Equal(HttpStatusCode.Created, r.StatusCode);
            return (await r.ReadAsync<CommentDto>()).Id;
        }

        var c1 = await CommentAsync();
        var c2 = await CommentAsync();
        var c3 = await CommentAsync();

        Assert.Equal(HttpStatusCode.Forbidden, (await stranger.DeleteAsync($"/api/recipes/{recipeId}/comments/{c1}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await commenter.DeleteAsync($"/api/recipes/{recipeId}/comments/{c1}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await recipeAuthor.DeleteAsync($"/api/recipes/{recipeId}/comments/{c2}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await admin.DeleteAsync($"/api/recipes/{recipeId}/comments/{c3}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.DeleteAsync($"/api/recipes/{recipeId}/comments/{c3}")).StatusCode);
    }

    [Fact]
    public async Task Empty_comment_is_rejected()
    {
        var (client, _) = await factory.NewUserAsync();
        var response = await client.PostAsJsonAsync("/api/recipes/1/comments", new CreateCommentRequest { Text = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("text", (await response.ReadAsync<ErrorResponse>()).Errors!.Keys);
    }

    [Fact]
    public async Task Admin_manages_lookups_and_duplicates_return_409()
    {
        var admin = await factory.LoginAsync(DbSeeder.AdminEmail, DbSeeder.AdminPassword);

        var created = await admin.PostAsJsonAsync("/api/ingredients", new { name = "ტარხუნა" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var duplicate = await admin.PostAsJsonAsync("/api/ingredients", new { name = "ტარხუნა" });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        var duplicateCategory = await admin.PostAsJsonAsync("/api/categories", new { name = "სუპები" });
        Assert.Equal(HttpStatusCode.Conflict, duplicateCategory.StatusCode);

        var search = await (await admin.GetAsync("/api/ingredients?search=ტარხ")).ReadAsync<PagedResult<object>>();
        Assert.Equal(1, search.TotalCount);
    }
}
