using System.Linq.Expressions;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IProductReadRepository : IRepository
{
    Task<TResult?> GetByIdAsync<TResult>(
        ProductId id,
        Expression<Func<Product, TResult>> selector,
        CancellationToken ct = default
    );

    Task<int> CountAsync(
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        bool hasDiscount = false,
        CancellationToken ct = default
    );

    Task<List<TResult>> GetPagedAsync<TResult>(
        int pageNumber,
        int pageSize,
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        ProductSortField sortBy,
        SortDirection direction,
        bool hasDiscount,
        Expression<Func<Product, TResult>> selector,
        CancellationToken ct = default
    );

    Task<List<Product>> GetByIdsAsync(List<ProductId> ids, CancellationToken ct = default);

    Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default);
}
