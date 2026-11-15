using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.Data.Seed;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>Applies pending migrations (creates the database if needed) and seeds demo data.</summary>
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db, hasher);
    }
}
