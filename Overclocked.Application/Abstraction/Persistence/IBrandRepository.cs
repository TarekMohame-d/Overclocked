using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IBrandRepository : IGenericRepository<Domain.BrandAggregate.Brand, BrandId>
{
    Task<Domain.BrandAggregate.Brand?> GetBrandByIdAsync(
        BrandId id,
        CancellationToken cancellationToken = default);

    Task<List<Domain.BrandAggregate.Brand>> GetBrandListAsync(CancellationToken cancellationToken = default);
}
