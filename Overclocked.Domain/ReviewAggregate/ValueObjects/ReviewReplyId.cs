using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ReviewAggregate.ValueObjects;

public record ReviewReplyId(Guid Value) : IEntityKey
{
    public static ReviewReplyId Create() => new(Guid.CreateVersion7());
    public static ReviewReplyId Create(Guid value) => new(value);
    public static implicit operator Guid(ReviewReplyId id) => id.Value;
}
