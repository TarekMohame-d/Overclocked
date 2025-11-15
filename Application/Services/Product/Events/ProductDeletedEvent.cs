using Application.Abstraction.Messaging;

namespace Application.Services.Product.Events;

public record ProductDeletedEvent(IEnumerable<string> OldImages) : IDomainEvent;
