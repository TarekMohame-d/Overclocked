using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IProductRepository : IGenericRepository<Domain.ProductAggregate.Product, ProductId>
{
}
