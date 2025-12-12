using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IProductRepository : IGenericRepository<ProductEntity, ProductId>
{
    Task<bool> ExistsAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetByIdWithDetailsAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        CancellationToken cancellationToken = default);
    Task<List<ProductEntity>> GetProductsPageAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        ProductSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default);

    Task<List<ProductEntity>> GetByIdsAsync(List<ProductId> ids, CancellationToken cancellationToken = default);
}
