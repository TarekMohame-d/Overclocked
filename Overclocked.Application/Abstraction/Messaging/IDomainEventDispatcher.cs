using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Abstraction.Messaging;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
