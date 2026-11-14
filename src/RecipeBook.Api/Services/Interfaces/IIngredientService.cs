using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Ingredients;

namespace RecipeBook.Api.Services.Interfaces;

public interface IIngredientService
{
    Task<PagedResult<IngredientDto>> GetAllAsync(SearchQuery query, CancellationToken ct);
    Task<IngredientDto> CreateAsync(CreateIngredientRequest request, CancellationToken ct);
}
