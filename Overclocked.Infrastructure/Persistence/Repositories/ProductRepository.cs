using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context)
    : GenericRepository<Product, ProductId>(context), IProductRepository
{
}
