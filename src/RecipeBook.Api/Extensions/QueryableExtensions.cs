using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.DTOs.Common;

namespace RecipeBook.Api.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Counts the filtered query, then fetches one page projected straight to the DTO
    /// (ProjectTo turns the AutoMapper map into a SQL SELECT — only needed columns are read).
    /// The query must already be ordered.
    /// </summary>
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        PaginationQuery paging,
        AutoMapper.IConfigurationProvider mapperConfig,
        CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ProjectTo<TDto>(mapperConfig)
            .ToListAsync(ct);

        return new PagedResult<TDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = paging.Page,
            PageSize = paging.PageSize
        };
    }
}
