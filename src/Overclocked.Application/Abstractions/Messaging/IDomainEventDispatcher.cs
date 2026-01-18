using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Abstractions.Messaging;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
