using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Favorites;

namespace RecipeBook.Api.Services.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteDto> AddAsync(int recipeId, CancellationToken ct);
    Task RemoveAsync(int recipeId, CancellationToken ct);
    Task<PagedResult<FavoriteDto>> GetMineAsync(PaginationQuery query, CancellationToken ct);
}
