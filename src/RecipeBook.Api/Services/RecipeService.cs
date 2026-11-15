using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Extensions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class RecipeService(
    AppDbContext db,
    IMapper mapper,
    ICurrentUserService currentUser) : IRecipeService
{
    /// <summary>How many ingredients a recipe may lack and still appear in "almost can cook".</summary>
    private const int MaxMissingIngredients = 2;

    public Task<PagedResult<RecipeListDto>> GetAllAsync(RecipeQueryParameters query, CancellationToken ct)
    {
        var recipes = db.Recipes.AsQueryable();

        if (query.CategoryId.HasValue)
            recipes = recipes.Where(r => r.CategoryId == query.CategoryId.Value);

        if (query.Difficulty.HasValue)
            recipes = recipes.Where(r => r.Difficulty == query.Difficulty.Value);

        if (query.MaxMinutes.HasValue)
            recipes = recipes.Where(r => r.CookingMinutes <= query.MaxMinutes.Value);

        if (query.IngredientId.HasValue)
            recipes = recipes.Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == query.IngredientId.Value));

        if (query.AuthorId.HasValue)
            recipes = recipes.Where(r => r.AuthorId == query.AuthorId.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            recipes = recipes.Where(r =>
                EF.Functions.Like(r.Title, pattern) ||
                (r.Description != null && EF.Functions.Like(r.Description, pattern)));
        }

        return recipes
            .OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
            .ToPagedResultAsync<Recipe, RecipeListDto>(query, mapper.ConfigurationProvider, ct);
    }

    public Task<PagedResult<RecipeListDto>> GetMineAsync(PaginationQuery query, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return db.Recipes
            .Where(r => r.AuthorId == userId)
            .OrderByDescending(r => r.CreatedAt).ThenByDescending(r => r.Id)
            .ToPagedResultAsync<Recipe, RecipeListDto>(query, mapper.ConfigurationProvider, ct);
    }

    public async Task<RecipeDetailsDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var dto = await db.Recipes
            .Where(r => r.Id == id)
            .ProjectTo<RecipeDetailsDto>(mapper.ConfigurationProvider)
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct)
            ?? throw NotFoundException.For("Recipe", id);

        // IsFavorite depends on who is asking, so it is filled in here and not by AutoMapper
        var userId = currentUser.UserId;
        dto.IsFavorite = await db.Favorites.AnyAsync(f => f.RecipeId == id && f.UserId == userId, ct);

        return dto;
    }

    public async Task<RecipeDetailsDto> CreateAsync(CreateRecipeRequest request, CancellationToken ct)
    {
        // AutoMapper builds the recipe together with its nested Steps and RecipeIngredients;
        // EF inserts all of them in one SaveChanges (one implicit transaction).
        var recipe = mapper.Map<Recipe>(request);
        recipe.AuthorId = currentUser.UserId;
        recipe.CreatedAt = DateTime.UtcNow;

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync(ct);

        return await GetByIdAsync(recipe.Id, ct);
    }

    public async Task<RecipeDetailsDto> UpdateAsync(int id, UpdateRecipeRequest request, CancellationToken ct)
    {
        var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct)
                     ?? throw NotFoundException.For("Recipe", id);

        EnsureCanModify(recipe.AuthorId);

        // Business rule 2: old steps and ingredient lines are fully replaced — in ONE transaction,
        // so a failure half-way can never leave a recipe with no steps.
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        await db.Steps.Where(s => s.RecipeId == id).ExecuteDeleteAsync(ct);
        await db.RecipeIngredients.Where(ri => ri.RecipeId == id).ExecuteDeleteAsync(ct);

        mapper.Map(request, recipe);           // scalar fields + new Steps / RecipeIngredients
        recipe.UpdatedAt = DateTime.UtcNow;    // business rule 5

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var recipe = await db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct)
                     ?? throw NotFoundException.For("Recipe", id);

        EnsureCanModify(recipe.AuthorId);

        // Steps, ingredient links, comments and favorites go with it (ON DELETE CASCADE)
        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync(ct);
    }

    public async Task<SearchByIngredientsResponse> SearchByIngredientsAsync(
        SearchByIngredientsRequest request, CancellationToken ct)
    {
        var have = request.IngredientIds.Distinct().ToList();

        // Recipes that use at least one ingredient I have and lack at most 2 others
        var candidates = db.Recipes.Where(r =>
            r.RecipeIngredients.Any(ri => have.Contains(ri.IngredientId)) &&
            r.RecipeIngredients.Count(ri => !have.Contains(ri.IngredientId)) <= MaxMissingIngredients);

        var matches = await candidates
            .OrderBy(r => r.Title)
            .ProjectTo<RecipeMatchDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        if (matches.Count == 0)
            return new SearchByIngredientsResponse();

        // Names of the missing ingredients for each candidate recipe
        var recipeIds = matches.Select(m => m.Id).ToList();
        var missing = await db.RecipeIngredients
            .Where(ri => recipeIds.Contains(ri.RecipeId) && !have.Contains(ri.IngredientId))
            .Select(ri => new { ri.RecipeId, ri.Ingredient.Name })
            .ToListAsync(ct);

        var missingByRecipe = missing.ToLookup(m => m.RecipeId, m => m.Name);
        foreach (var match in matches)
            match.MissingIngredients = missingByRecipe[match.Id].Order().ToList();

        return new SearchByIngredientsResponse
        {
            CanCook = matches.Where(m => m.MissingIngredients.Count == 0).ToList(),
            AlmostCanCook = matches
                .Where(m => m.MissingIngredients.Count > 0)
                .OrderBy(m => m.MissingIngredients.Count).ThenBy(m => m.Title)
                .ToList()
        };
    }

    /// <summary>Business rule 1 (ownership): only the author or an Admin may change a recipe.</summary>
    private void EnsureCanModify(int authorId)
    {
        if (authorId != currentUser.UserId && !currentUser.IsAdmin)
            throw new ForbiddenException("Only the recipe's author or an admin can modify it.");
    }
}
