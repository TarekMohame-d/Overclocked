using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Domain.TagAggregate;

public sealed class Tag : AggregateRoot<TagId>
{
    private Tag()
    {
    }

    private Tag(TagId id, string name) : base(id)
    {
        Name = name;

        NormalizedName = name.ToUpper();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Tag Create(string name) =>
        new(id: TagId.Create(), name: name);

    public void Update(string name)
    {
        Name = name;
        NormalizedName = name.ToUpper();
        UpdatedAt = DateTime.UtcNow;
    }
}
