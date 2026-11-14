using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Ingredients;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
[Authorize]
[Produces("application/json")]
public class IngredientsController(IIngredientService ingredientService) : ControllerBase
{
    /// <summary>Ingredient lookup list, filterable with ?search=.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<IngredientDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<IngredientDto>>> GetAll([FromQuery] SearchQuery query, CancellationToken ct)
    {
        return Ok(await ingredientService.GetAllAsync(query, ct));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType<IngredientDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IngredientDto>> Create(CreateIngredientRequest request, CancellationToken ct)
    {
        var ingredient = await ingredientService.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, ingredient);
    }
}
