using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.Events;

public record UserRegisteredEvent(string Email, string Code) : IDomainEvent;
