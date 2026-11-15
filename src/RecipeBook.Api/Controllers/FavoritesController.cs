using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.DTOs.Favorites;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "User")]
[Produces("application/json")]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    /// <summary>Adds a recipe to my favorites (409 if it is already there).</summary>
    [HttpPost("recipes/{recipeId:int}/favorite")]
    [ProducesResponseType<FavoriteDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FavoriteDto>> Add(int recipeId, CancellationToken ct)
    {
        var favorite = await favoriteService.AddAsync(recipeId, ct);
        return StatusCode(StatusCodes.Status201Created, favorite);
    }

    [HttpDelete("recipes/{recipeId:int}/favorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(int recipeId, CancellationToken ct)
    {
        await favoriteService.RemoveAsync(recipeId, ct);
        return NoContent();
    }

    [HttpGet("me/favorites")]
    [ProducesResponseType<PagedResult<FavoriteDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<FavoriteDto>>> GetMine([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        return Ok(await favoriteService.GetMineAsync(query, ct));
    }
}
