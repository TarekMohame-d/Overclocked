using Application.Features.Product.Queries.GetProductById;
using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<ProductDto?> GetProductAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<Product> GetProductsQuery(string? sortBy);
}
