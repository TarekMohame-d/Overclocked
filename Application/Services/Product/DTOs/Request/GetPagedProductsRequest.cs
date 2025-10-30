using System.Text.Json.Serialization;
using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Enums;

namespace Application.Services.Product.DTOs.Request;

public record GetPagedProductsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductSortField SortBy { get; set; } = ProductSortField.Id;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection Direction { get; set; } = SortDirection.Asc;

    public string? Search { get; set; } = null;
    public string? Category { get; set; } = null;
    public string? Brand { get; set; } = null;
    public Guid? TagId { get; set; } = null;
}

public record GetPagedProductsQuery : GetPagedProductsRequest, ICachedRequest
{
    public string CacheKey => CacheKeys.ProductPaged(
        Page, PageSize, SortBy.ToString(), Direction.ToString(),
        Category ?? "all", Brand ?? "all", TagId?.ToString() ?? "all", Search ?? "all"
    );

    public string CacheSetKey => CacheKeys.ProductSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);

    public static GetPagedProductsQuery FromRequest(GetPagedProductsRequest request)
    {
        return new GetPagedProductsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            Direction = request.Direction,
            Search = request.Search,
            Category = request.Category,
            Brand = request.Brand,
            TagId = request.TagId
        };
    }
}
