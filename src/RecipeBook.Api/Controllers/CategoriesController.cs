using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Categories;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetAll([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        return Ok(await categoryService.GetAllAsync(query, ct));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var category = await categoryService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, category);
    }
}
