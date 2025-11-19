using Application.Abstraction.Messaging;

namespace Application.Services.Brand.Events;

public record BrandDeletedEvent(string ImageUrl) : IDomainEvent;
