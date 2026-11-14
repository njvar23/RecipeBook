namespace RecipeBook.Api.DTOs.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public int UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;   // User.DisplayName
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
