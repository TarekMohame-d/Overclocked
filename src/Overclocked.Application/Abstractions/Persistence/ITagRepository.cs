using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ITagRepository : IRepository
{
    Task<Tag?> GetByIdAsync(TagId id, CancellationToken ct = default);

    Task<bool> ExistsAsync(TagId id, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);

    void Add(Tag tag);

    void Remove(Tag tag);
}
