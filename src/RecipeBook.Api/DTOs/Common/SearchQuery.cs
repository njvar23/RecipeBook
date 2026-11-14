namespace RecipeBook.Api.DTOs.Common;

/// <summary>Pagination plus an optional ?search= text filter.</summary>
public class SearchQuery : PaginationQuery
{
    public string? Search { get; set; }
}
