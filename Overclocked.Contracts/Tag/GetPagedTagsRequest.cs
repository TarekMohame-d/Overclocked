namespace Overclocked.Contracts.Tag;

public record GetPagedTagsRequest
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
    public required string SearchTerm { get; init; } = string.Empty;
    public required string SortBy { get; init; } = "id";
    public required string Direction { get; init; } = "asc";
}
