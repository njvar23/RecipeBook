using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RecipeBook.Api.DTOs.Auth;

namespace RecipeBook.Tests.Infrastructure;

public static class ApiClient
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task<T> ReadAsync<T>(this HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(Json);
        Assert.NotNull(value);
        return value!;
    }

    /// <summary>Logs in and returns a client that sends the Bearer token.</summary>
    public static async Task<HttpClient> LoginAsync(this RecipeBookApiFactory factory, string email, string password)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Email = email, Password = password });
        response.EnsureSuccessStatusCode();

        var auth = await response.ReadAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        return client;
    }

    /// <summary>Registers a brand-new user (unique email) and returns a logged-in client plus the user id.</summary>
    public static async Task<(HttpClient Client, int UserId)> NewUserAsync(this RecipeBookApiFactory factory, string name = "Tester")
    {
        var email = $"user-{Guid.NewGuid():N}@test.ge";
        const string password = "Password1";

        var anonymous = factory.CreateClient();
        var response = await anonymous.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest { Email = email, Password = password, DisplayName = name });
        response.EnsureSuccessStatusCode();
        var user = await response.ReadAsync<UserDto>();

        return (await factory.LoginAsync(email, password), user.Id);
    }
}
