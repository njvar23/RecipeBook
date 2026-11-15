using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Favorites;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Extensions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class FavoriteService(
    AppDbContext db,
    IMapper mapper,
    ICurrentUserService currentUser) : IFavoriteService
{
    public async Task<FavoriteDto> AddAsync(int recipeId, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var recipe = await db.Recipes
            .Where(r => r.Id == recipeId)
            .Select(r => new { r.AuthorId })
            .FirstOrDefaultAsync(ct)
            ?? throw NotFoundException.For("Recipe", recipeId);

        // Favorites are for saving other people's recipes
        if (recipe.AuthorId == userId)
            throw new BusinessRuleException("You cannot add your own recipe to favorites.");

        if (await db.Favorites.AnyAsync(f => f.UserId == userId && f.RecipeId == recipeId, ct))
            throw new ConflictException("This recipe is already in your favorites.");

        db.Favorites.Add(new Favorite { UserId = userId, RecipeId = recipeId, AddedAt = DateTime.UtcNow });
        await db.SaveChangesAsync(ct);

        return await db.Favorites
            .Where(f => f.UserId == userId && f.RecipeId == recipeId)
            .ProjectTo<FavoriteDto>(mapper.ConfigurationProvider)
            .FirstAsync(ct);
    }

    public async Task RemoveAsync(int recipeId, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var favorite = await db.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId, ct)
                       ?? throw new NotFoundException($"Recipe {recipeId} is not in your favorites.");

        db.Favorites.Remove(favorite);
        await db.SaveChangesAsync(ct);
    }

    public Task<PagedResult<FavoriteDto>> GetMineAsync(PaginationQuery query, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return db.Favorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.AddedAt).ThenByDescending(f => f.RecipeId)
            .ToPagedResultAsync<Favorite, FavoriteDto>(query, mapper.ConfigurationProvider, ct);
    }
}
