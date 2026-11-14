using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.ChefProfiles;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api")]
[Authorize]
[Produces("application/json")]
public class ChefProfilesController(IChefProfileService chefProfileService) : ControllerBase
{
    /// <summary>My chef profile (404 if I have not created one).</summary>
    [HttpGet("me/chef-profile")]
    [Authorize(Roles = "User")]
    [ProducesResponseType<ChefProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefProfileDto>> GetMine(CancellationToken ct)
    {
        return Ok(await chefProfileService.GetMineAsync(ct));
    }

    /// <summary>Creates (201) or updates (200) my chef profile.</summary>
    [HttpPut("me/chef-profile")]
    [Authorize(Roles = "User")]
    [ProducesResponseType<ChefProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ChefProfileDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ChefProfileDto>> UpsertMine(UpsertChefProfileRequest request, CancellationToken ct)
    {
        var (profile, created) = await chefProfileService.UpsertMineAsync(request, ct);
        return created ? CreatedAtAction(nameof(GetMine), profile) : Ok(profile);
    }

    /// <summary>Someone else's chef profile with the number of recipes they have published.</summary>
    [HttpGet("users/{userId:int}/chef-profile")]
    [ProducesResponseType<ChefProfileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChefProfileDto>> GetByUser(int userId, CancellationToken ct)
    {
        return Ok(await chefProfileService.GetByUserIdAsync(userId, ct));
    }
}
