using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record ProductTag : IValueObject
{
    public TagId TagId { get; private set; } = null!;
    public Tag? Tag { get; }

    private ProductTag() { }

    private ProductTag(TagId tagId) => TagId = tagId;

    public static ProductTag Create(TagId tagId) => new(tagId);
}
