using Application.Abstraction.Messaging;

namespace Application.Services.Brand.Events;

public record BrandUpdatedEvent(string ImageUrl) : IDomainEvent;
