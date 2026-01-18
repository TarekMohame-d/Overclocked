namespace Overclocked.Application.Features.ProductUseCases.DTOs.Requests;

public record GetPagedProductsQuery(
    int? Page,
    int? PageSize,
    string? SearchTerm,
    string? SortBy,
    string? Direction,
    bool? HasDiscount,
    Guid? CategoryId,
    Guid? BrandId,
    Guid? TagId
);
