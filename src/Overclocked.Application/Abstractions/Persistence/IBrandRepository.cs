using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IBrandRepository : IRepository
{
    Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default);

    Task<bool> ExistsAsync(BrandId id, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);

    void Add(Brand brand);

    void Remove(Brand brand);
}
