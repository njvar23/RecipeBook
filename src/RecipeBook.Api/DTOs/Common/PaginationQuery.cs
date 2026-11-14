namespace RecipeBook.Api.DTOs.Common;

/// <summary>?page=1&amp;pageSize=20 — bound from the query string.</summary>
public class PaginationQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
}
