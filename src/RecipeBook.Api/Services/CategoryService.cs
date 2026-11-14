using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Categories;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Extensions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class CategoryService(AppDbContext db, IMapper mapper) : ICategoryService
{
    public Task<PagedResult<CategoryDto>> GetAllAsync(PaginationQuery query, CancellationToken ct) =>
        db.Categories
            .OrderBy(c => c.Name)
            .ToPagedResultAsync<Category, CategoryDto>(query, mapper.ConfigurationProvider, ct);

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct))
            throw new ConflictException($"Category '{name}' already exists.");

        var category = mapper.Map<Category>(request);
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        return mapper.Map<CategoryDto>(category); // RecipesCount = 0 for a new category
    }
}
