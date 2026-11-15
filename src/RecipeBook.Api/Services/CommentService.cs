using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.DTOs.Comments;
using RecipeBook.Api.Entities;
using RecipeBook.Api.Exceptions;
using RecipeBook.Api.Services.Interfaces;

namespace RecipeBook.Api.Services;

public class CommentService(
    AppDbContext db,
    IMapper mapper,
    ICurrentUserService currentUser) : ICommentService
{
    public async Task<CommentDto> CreateAsync(int recipeId, CreateCommentRequest request, CancellationToken ct)
    {
        if (!await db.Recipes.AnyAsync(r => r.Id == recipeId, ct))
            throw NotFoundException.For("Recipe", recipeId);

        var comment = mapper.Map<Comment>(request);
        comment.RecipeId = recipeId;
        comment.UserId = currentUser.UserId;
        comment.CreatedAt = DateTime.UtcNow;

        db.Comments.Add(comment);
        await db.SaveChangesAsync(ct);

        return await db.Comments
            .Where(c => c.Id == comment.Id)
            .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
            .FirstAsync(ct);
    }

    public async Task DeleteAsync(int recipeId, int commentId, CancellationToken ct)
    {
        var comment = await db.Comments
            .Include(c => c.Recipe)
            .FirstOrDefaultAsync(c => c.Id == commentId && c.RecipeId == recipeId, ct)
            ?? throw new NotFoundException($"Comment {commentId} was not found on recipe {recipeId}.");

        // Business rule 4: the comment's author, the recipe's author or an Admin
        var userId = currentUser.UserId;
        var allowed = comment.UserId == userId
                      || comment.Recipe.AuthorId == userId
                      || currentUser.IsAdmin;

        if (!allowed)
            throw new ForbiddenException("Only the comment's author, the recipe's author or an admin can delete this comment.");

        db.Comments.Remove(comment);
        await db.SaveChangesAsync(ct);
    }
}
