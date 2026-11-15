using RecipeBook.Api.DTOs.Comments;

namespace RecipeBook.Api.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateAsync(int recipeId, CreateCommentRequest request, CancellationToken ct);
    Task DeleteAsync(int recipeId, int commentId, CancellationToken ct);
}
