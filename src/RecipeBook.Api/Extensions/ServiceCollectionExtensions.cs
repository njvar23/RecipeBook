using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Filters;
using RecipeBook.Api.Infrastructure;
using RecipeBook.Api.Options;
using RecipeBook.Api.Services;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // The connection string is read when the context is created, so tests can override it
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");
            options.UseSqlite(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
                // Without this, MVC treats every non-nullable string as [Required] and answers
                // before FluentValidation runs — then the client would not see all errors at once.
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .ConfigureApiBehaviorOptions(options =>
            {
                // Malformed JSON / wrong types / bad enum in the query string → same error format
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value is { Errors.Count: > 0 })
                        .ToDictionary(
                            e => ErrorKeys.ToCamelCasePath(e.Key),
                            e => e.Value!.Errors.Select(x =>
                                string.IsNullOrEmpty(x.ErrorMessage) ? "The value is invalid." : x.ErrorMessage).ToArray());

                    return new BadRequestObjectResult(
                        new ErrorResponse(StatusCodes.Status400BadRequest, "Validation failed", errors));
                };
            });

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddAutoMapper(typeof(Program).Assembly);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IChefProfileService, ChefProfileService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IIngredientService, IngredientService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IFavoriteService, FavoriteService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Key) && s.Key.Length >= 32,
                "Jwt:Key must be at least 32 characters long.")
            .Validate(s => s.ExpiresInHours is >= 1 and <= 24,
                "Jwt:ExpiresInHours must be between 1 and 24.")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Issuer) && !string.IsNullOrWhiteSpace(s.Audience),
                "Jwt:Issuer and Jwt:Audience are required.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        // Configure the bearer handler from the validated JwtSettings (key comes from appsettings, not code)
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                options.MapInboundClaims = false; // keep "sub", "role" as they are in the token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    NameClaimType = AppClaimTypes.Name,
                    RoleClaimType = AppClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                // 401 and 403 also use the unified error body
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new ErrorResponse(
                            StatusCodes.Status401Unauthorized,
                            "Authentication is required. Send a valid token in the Authorization: Bearer header."));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new ErrorResponse(
                            StatusCodes.Status403Forbidden,
                            "Your role is not allowed to use this endpoint."));
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RecipeBook API",
                Version = "v1",
                Description = "Recipe platform: recipes with steps and ingredients, comments, favorites and chef profiles. " +
                              "Log in with POST /api/auth/login, then press Authorize and paste the accessToken."
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste only the token (without the word Bearer)."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
