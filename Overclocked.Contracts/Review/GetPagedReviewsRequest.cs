namespace Overclocked.Contracts.Review;

public record GetPagedReviewsRequest
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? SortBy { get; init; }
    public string? Direction { get; init; }
}
