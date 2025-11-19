using Application.Abstraction.Messaging;

namespace Application.Services.Category.Events;

public record CategoryUpdatedEvent(string ImageUrl) : IDomainEvent;
