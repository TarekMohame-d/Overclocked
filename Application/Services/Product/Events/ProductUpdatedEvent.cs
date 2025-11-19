using Application.Abstraction.Messaging;

namespace Application.Services.Product.Events;

public record ProductUpdatedEvent(IEnumerable<string> OldImages) : IDomainEvent;
