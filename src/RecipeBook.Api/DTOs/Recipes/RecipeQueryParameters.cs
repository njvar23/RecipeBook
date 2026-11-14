using RecipeBook.Api.DTOs.Common;
using RecipeBook.Api.Entities;

namespace RecipeBook.Api.DTOs.Recipes;

/// <summary>GET /api/recipes?categoryId=&amp;difficulty=&amp;maxMinutes=&amp;ingredientId=&amp;authorId=&amp;search=&amp;page=&amp;pageSize=</summary>
public class RecipeQueryParameters : PaginationQuery
{
    public int? CategoryId { get; set; }
    public Difficulty? Difficulty { get; set; }
    public int? MaxMinutes { get; set; }
    public int? IngredientId { get; set; }
    public int? AuthorId { get; set; }
    public string? Search { get; set; }
}
