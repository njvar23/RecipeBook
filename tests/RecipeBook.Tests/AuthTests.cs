using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.DTOs.Auth;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Tests.Infrastructure;

namespace RecipeBook.Tests;

public class AuthTests(RecipeBookApiFactory factory) : IClassFixture<RecipeBookApiFactory>
{
    [Fact]
    public async Task Register_then_login_returns_token_with_sub_email_and_role()
    {
        var client = factory.CreateClient();
        var email = $"new-{Guid.NewGuid():N}@test.ge";

        var register = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = email, Password = "Secret123", DisplayName = "ლუკა" });
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);
        var user = await register.ReadAsync<UserDto>();
        Assert.Equal("User", user.Role);

        var login = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Email = email.ToUpperInvariant(), Password = "Secret123" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var auth = await login.ReadAsync<AuthResponse>();

        Assert.True(auth.ExpiresAt > DateTime.UtcNow.AddMinutes(50));
        var token = new JwtSecurityTokenHandler().ReadJwtToken(auth.AccessToken);
        Assert.Equal(user.Id.ToString(), token.Claims.Single(c => c.Type == "sub").Value);
        Assert.Equal(email, token.Claims.Single(c => c.Type == "email").Value);
        Assert.Equal("User", token.Claims.Single(c => c.Type == "role").Value);
    }

    [Fact]
    public async Task Register_with_bad_data_returns_every_error_at_once()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = "not-an-email", Password = "short", DisplayName = "x" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.ReadAsync<ErrorResponse>();
        Assert.Equal("Validation failed", error.Message);
        Assert.Contains("email", error.Errors!.Keys);
        Assert.Contains("displayName", error.Errors.Keys);
        Assert.Equal(2, error.Errors["password"].Length); // too short AND no digit
    }

    [Fact]
    public async Task Register_with_existing_email_is_rejected()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = DbSeeder.NinoEmail, Password = "Password1", DisplayName = "Copy" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.ReadAsync<ErrorResponse>();
        Assert.Contains("A user with this email already exists.", error.Errors!["email"]);
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest { Email = DbSeeder.NinoEmail, Password = "Wrong1234" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Protected_endpoint_without_token_returns_401_in_unified_format()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/recipes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var error = await response.ReadAsync<ErrorResponse>();
        Assert.Equal(401, error.Status);
    }

    [Fact]
    public async Task Wrong_role_returns_403()
    {
        var user = await factory.LoginAsync(DbSeeder.NinoEmail, DbSeeder.NinoPassword);
        var admin = await factory.LoginAsync(DbSeeder.AdminEmail, DbSeeder.AdminPassword);

        var userCreatesCategory = await user.PostAsJsonAsync("/api/categories", new { name = "ახალი" });
        var adminCreatesRecipe = await admin.PostAsJsonAsync("/api/recipes", new { title = "x" });

        Assert.Equal(HttpStatusCode.Forbidden, userCreatesCategory.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, adminCreatesRecipe.StatusCode);
    }

    [Fact]
    public async Task Unknown_route_returns_404_in_unified_format()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.ReadAsync<ErrorResponse>();
        Assert.Equal(404, error.Status);
    }
}
