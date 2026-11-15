using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase()
    .AddApiControllers()
    .AddApplicationServices()
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt();

var app = builder.Build();

app.UseExceptionHandler();

// Empty-body 4xx responses (unknown route 404, 405 ...) also get the unified error format
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    var message = response.StatusCode switch
    {
        StatusCodes.Status404NotFound => "The requested endpoint does not exist.",
        StatusCodes.Status405MethodNotAllowed => "This HTTP method is not allowed for this endpoint.",
        _ => "The request could not be processed."
    };
    await response.WriteAsJsonAsync(new ErrorResponse(response.StatusCode, message));
});

app.UseSwagger();
app.UseSwaggerUI(c => c.DocumentTitle = "RecipeBook API");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.MigrateAndSeedAsync();

app.Run();

// Makes Program visible to WebApplicationFactory<Program> in the test project
public partial class Program;
