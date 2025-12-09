using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.UserAggregate.Events;

public record UserEmailConfirmationCodeResendEvent(string Email, string Code) : IDomainEvent;
