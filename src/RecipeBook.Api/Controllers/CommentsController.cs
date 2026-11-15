using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBook.Api.DTOs.Comments;
using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Controllers;

[ApiController]
[Route("api/recipes/{recipeId:int}/comments")]
[Authorize]
[Produces("application/json")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "User")]
    [ProducesResponseType<CommentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Create(int recipeId, CreateCommentRequest request, CancellationToken ct)
    {
        var comment = await commentService.CreateAsync(recipeId, request, ct);
        return StatusCode(StatusCodes.Status201Created, comment);
    }

    /// <summary>Allowed for the comment's author, the recipe's author or an Admin.</summary>
    [HttpDelete("{commentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int recipeId, int commentId, CancellationToken ct)
    {
        await commentService.DeleteAsync(recipeId, commentId, ct);
        return NoContent();
    }
}
