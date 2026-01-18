using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.Events;

public record UserEmailConfirmedEvent(Guid UserId) : IDomainEvent;
