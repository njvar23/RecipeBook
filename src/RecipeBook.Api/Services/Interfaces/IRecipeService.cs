using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Recipes;

namespace RecipeBook.Api.Services.Interfaces;

public interface IRecipeService
{
    Task<PagedResult<RecipeListDto>> GetAllAsync(RecipeQueryParameters query, CancellationToken ct);
    Task<PagedResult<RecipeListDto>> GetMineAsync(PaginationQuery query, CancellationToken ct);
    Task<RecipeDetailsDto> GetByIdAsync(int id, CancellationToken ct);
    Task<RecipeDetailsDto> CreateAsync(CreateRecipeRequest request, CancellationToken ct);
    Task<RecipeDetailsDto> UpdateAsync(int id, UpdateRecipeRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<SearchByIngredientsResponse> SearchByIngredientsAsync(SearchByIngredientsRequest request, CancellationToken ct);
}
