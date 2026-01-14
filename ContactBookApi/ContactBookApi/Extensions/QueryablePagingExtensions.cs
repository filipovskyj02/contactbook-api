using ContactBookApi.Dtos.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ContactBookApi.Extensions;

public static class QueryablePagingExtensions
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(
        this IQueryable<T> query,
        PaginationParams pagination,
        CancellationToken ct)
    {
        var page = pagination.SafePage;
        var pageSize = pagination.SafePageSize;

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}