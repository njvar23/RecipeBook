using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Recipes;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
[Produces("application/json")]
public class RecipesController(IRecipeService recipeService) : ControllerBase
{
    /// <summary>Recipe list with filters and pagination.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<RecipeListDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<RecipeListDto>>> GetAll(
        [FromQuery] RecipeQueryParameters query, CancellationToken ct)
    {
        return Ok(await recipeService.GetAllAsync(query, ct));
    }

    /// <summary>Full recipe: ordered steps, ingredients, author and comments.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<RecipeDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailsDto>> GetById(int id, CancellationToken ct)
    {
        return Ok(await recipeService.GetByIdAsync(id, ct));
    }

    /// <summary>Creates a recipe together with its steps and ingredients in one request.</summary>
    [HttpPost]
    [Authorize(Roles = "User")]
    [ProducesResponseType<RecipeDetailsDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDetailsDto>> Create(CreateRecipeRequest request, CancellationToken ct)
    {
        var recipe = await recipeService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    /// <summary>Full update; steps and ingredients are replaced. Author or Admin only.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<RecipeDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDetailsDto>> Update(int id, UpdateRecipeRequest request, CancellationToken ct)
    {
        return Ok(await recipeService.UpdateAsync(id, request, ct));
    }

    /// <summary>Deletes the recipe with its steps, ingredient links, comments and favorites. Author or Admin only.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await recipeService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>Recipes I have written.</summary>
    [HttpGet("/api/me/recipes")]
    [Authorize(Roles = "User")]
    [ProducesResponseType<PagedResult<RecipeListDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RecipeListDto>>> GetMine([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        return Ok(await recipeService.GetMineAsync(query, ct));
    }
}
