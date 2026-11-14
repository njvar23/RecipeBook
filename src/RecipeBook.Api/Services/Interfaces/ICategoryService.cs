using RecipeBook.Api.DTOs.Categories;
using RecipeBook.Api.DTOs.Common;

namespace RecipeBook.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<PagedResult<CategoryDto>> GetAllAsync(PaginationQuery query, CancellationToken ct);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct);
}
