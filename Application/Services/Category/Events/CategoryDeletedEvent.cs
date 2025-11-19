using Application.Abstraction.Messaging;

namespace Application.Services.Category.Events;

public record CategoryDeletedEvent(string ImageUrl) : IDomainEvent;
