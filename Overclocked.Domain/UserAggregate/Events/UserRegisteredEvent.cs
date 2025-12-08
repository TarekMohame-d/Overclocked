using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.UserAggregate.Events;

public record UserRegisteredEvent(string Email, string Code) : IDomainEvent;
