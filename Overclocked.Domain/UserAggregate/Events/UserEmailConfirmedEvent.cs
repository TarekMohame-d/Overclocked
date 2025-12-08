using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.UserAggregate.Events;

public record UserEmailConfirmedEvent(Guid UserId) : IDomainEvent;
