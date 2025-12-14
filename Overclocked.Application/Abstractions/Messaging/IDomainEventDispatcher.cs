using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
