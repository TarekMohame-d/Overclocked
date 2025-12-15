using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IProductRepository : IGenericRepository<ProductEntity, ProductId>
{
    Task<ProductEntity?> FetchPrimitiveAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> GetForUpdateAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<ProductEntity?> FindAsync(ProductId id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        CancellationToken cancellationToken = default);
    Task<List<ProductEntity>> GetPagedAsync(
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
