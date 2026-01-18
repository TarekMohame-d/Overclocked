using System.Text.Json.Serialization;

namespace Overclocked.SharedKernel;

public sealed class PagedResult<T>
{
    [JsonConstructor]
    private PagedResult(
        IEnumerable<T> items,
        int pageNumber,
        int pageSize,
        int totalItemCount,
        int totalPageCount,
        bool hasPreviousPage,
        bool hasNextPage
    )
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
        var totalPages = totalItemCount == 0 ? 0 : (int)Math.Ceiling(totalItemCount / (double)pageSize);

        var hasPreviousPage = pageNumber > 1 && pageNumber <= totalPages;
        var hasNextPage = pageNumber < totalPages;

        return new PagedResult<T>(
            items: items,
            pageNumber: pageNumber,
            pageSize: pageSize,
            totalItemCount: totalItemCount,
            totalPageCount: totalPages,
            hasPreviousPage: hasPreviousPage,
            hasNextPage: hasNextPage
        );
    }

    public static PagedResult<T> Empty(int pageNumber, int pageSize) => Create([], pageNumber, pageSize, 0);
}
