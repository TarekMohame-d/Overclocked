using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductsAggregate.ValueObjects;

namespace Overclocked.Domain.ProductsAggregate.Entities;

public sealed class Specification : Entity<SpecificationId>
{
    private Specification()
    {
    }

    private Specification(SpecificationId id, string name, string value) : base(id)
    {
        Name = name;
        Value = value;

        NormalizedName = name.ToUpper();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string NormalizedName { get; } = string.Empty;
    public string Value { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    internal static Specification Create(SpecificationId id, string name, string value) =>
        new(id: id, name: name, value: value);
}
