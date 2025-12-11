using Overclocked.Application.Common.Enums;
using Overclocked.Contracts.Product;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IProductRepository : IGenericRepository<ProductEntity, ProductId>
{
    Task<ProductEntity?> GetByIdWithDetailsAsync(ProductId id, CancellationToken cancellationToken);
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
}
