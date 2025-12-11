namespace Overclocked.Contracts.Product;

public record GetPagedProductsRequest
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public string? Direction { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public Guid? TagId { get; init; }
}
