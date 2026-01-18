using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IProductRepository : IRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default);

    Task<List<Product>> GetByIdsAsync(List<ProductId> ids, CancellationToken ct = default);

    Task<Product?> FindAsync(ProductId id, CancellationToken ct = default);

    Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);

    void Add(Product product);

    void Remove(Product product);
}
