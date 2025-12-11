using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IProductRepository : IGenericRepository<ProductEntity, ProductId>
{
    Task<ProductEntity?> GetByIdWithDetailsAsync(ProductId id, CancellationToken cancellationToken);
}
