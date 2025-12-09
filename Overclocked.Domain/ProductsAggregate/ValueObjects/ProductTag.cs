using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Domain.ProductsAggregate.ValueObjects;

public record ProductTag : IValueObject
{
    public TagId TagId { get; private set; }

    private ProductTag()
    {
    }

    private ProductTag(TagId tagId)
    {
        TagId = tagId;
    }

    public static ProductTag Create(TagId tagId) => new(tagId);
}
