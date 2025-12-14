using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IBrandRepository : IGenericRepository<Domain.BrandAggregate.Brand, BrandId>
{
    Task<Domain.BrandAggregate.Brand?> GetByIdAsync(
        BrandId id,
        CancellationToken cancellationToken = default);

    Task<List<Domain.BrandAggregate.Brand>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Domain.BrandAggregate.Brand?> FindAsync(BrandId id, CancellationToken cancellationToken = default);
}
