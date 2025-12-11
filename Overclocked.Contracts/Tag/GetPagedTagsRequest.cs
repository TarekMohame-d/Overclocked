namespace Overclocked.Contracts.Tag;

public record GetPagedTagsRequest
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public string? Direction { get; init; }
}
