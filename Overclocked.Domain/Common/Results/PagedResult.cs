using System.Text.Json.Serialization;

namespace Overclocked.Domain.Common.Results;

public class PagedResult<T>
{
    [JsonConstructor]
    private PagedResult(
        IEnumerable<T> items,
        int pageNumber,
        int pageSize,
        int totalItemCount,
        int totalPageCount,
        bool hasPreviousPage,
        bool hasNextPage)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItemCount = totalItemCount;
        TotalPageCount = totalPageCount;
        HasPreviousPage = hasPreviousPage;
        HasNextPage = hasNextPage;
    }

    public IEnumerable<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalItemCount { get; }
    public int TotalPageCount { get; }
    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }

    public static PagedResult<T> Create(IEnumerable<T> items, int pageNumber, int pageSize, int totalItemCount)
    {
        var totalPages = (int)Math.Ceiling((double)totalItemCount / pageSize);

        return new PagedResult<T>(
            items: items,
            pageNumber: pageNumber,
            pageSize: pageSize,
            totalItemCount: totalItemCount,
            totalPageCount: totalPages,
            hasPreviousPage: pageNumber > 1,
            hasNextPage: pageNumber < totalPages
        );
    }

    public static PagedResult<T> Empty(int pageNumber, int pageSize)
    {
        return Create([], pageNumber, pageSize, 0);
    }
}
