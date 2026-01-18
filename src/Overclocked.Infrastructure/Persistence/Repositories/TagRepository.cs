using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext dbContext) : ITagRepository
{
    private readonly DbSet<Tag> _dbSet = dbContext.Tags;

    public Task<Tag?> GetByIdAsync(TagId id, CancellationToken ct = default) =>
        _dbSet.FindAsync([id], cancellationToken: ct).AsTask();

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
    {
        var normalizedInput = name.Trim().ToUpperInvariant();
        return _dbSet.AnyAsync(x => x.NormalizedName == normalizedInput, ct);
    }

    public Task<bool> ExistsAsync(TagId id, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.Id == id, ct);

    public void Add(Tag tag) => _dbSet.Add(tag);

    public void Remove(Tag tag) => _dbSet.Remove(tag);
}
