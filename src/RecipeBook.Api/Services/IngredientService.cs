using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Ingredients;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Extensions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class IngredientService(AppDbContext db, IMapper mapper) : IIngredientService
{
    public Task<PagedResult<IngredientDto>> GetAllAsync(SearchQuery query, CancellationToken ct)
    {
        var ingredients = db.Ingredients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            ingredients = ingredients.Where(i => EF.Functions.Like(i.Name, pattern));
        }

        return ingredients
            .OrderBy(i => i.Name)
            .ToPagedResultAsync<Ingredient, IngredientDto>(query, mapper.ConfigurationProvider, ct);
    }

    public async Task<IngredientDto> CreateAsync(CreateIngredientRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        if (await db.Ingredients.AnyAsync(i => i.Name.ToLower() == name.ToLower(), ct))
            throw new ConflictException($"Ingredient '{name}' already exists.");

        var ingredient = mapper.Map<Ingredient>(request);
        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync(ct);

        return mapper.Map<IngredientDto>(ingredient);
    }
}
