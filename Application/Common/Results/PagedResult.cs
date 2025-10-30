using Microsoft.EntityFrameworkCore;

public class PagedResult<T>
{
    // Needed for deserialization
    public PagedResult() { }
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItemCount { get; set; }
    public int TotalPageCount => (int)Math.Ceiling((double)TotalItemCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPageCount;

    private PagedResult(List<T> items, int pageNumber, int pageSize, int totalItemCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItemCount = totalItemCount;
    }

    // Static factory method to create the paged result
    public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> source, int pageNumber = 1, int pageSize = 10)
    {
        var count = await source.CountAsync();

        var items = await source.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new PagedResult<T>(items, pageNumber, pageSize, count);
    }
}
