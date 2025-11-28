using Application.Common.Enums;
using Application.Services.Product.DTOs.Response;
using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetProductForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithImagesAsync(Guid id, CancellationToken cancellationToken = default);

    IQueryable<Product> GetProductsQuery(
        ProductSortField sortBy,
        SortDirection direction,
        string? search = null,
        string? category = null,
        string? brand = null,
        Guid? tagId = null
    );

    Task<int?> GetProductStockQuantityAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
