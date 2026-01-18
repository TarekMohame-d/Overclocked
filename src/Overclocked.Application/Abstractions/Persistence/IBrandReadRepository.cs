using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IBrandReadRepository : IRepository
{
    Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default);

    Task<List<Brand>> GetAllAsync(CancellationToken ct = default);
}
