using Microsoft.EntityFrameworkCore;

namespace LekkoApp.Helpers;

public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int PageElementsStart { get; }
    public int PageElementsEnd { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        TotalCount = count;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        if (count == 0)
        {
            PageElementsStart = 0;
            PageElementsEnd = 0;
        }
        else
        {
            PageElementsStart = (pageIndex - 1) * pageSize + 1;
            PageElementsEnd = Math.Min(pageIndex * pageSize, count);
        }

        AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize)
    {
        var count = await source.CountAsync();

        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}