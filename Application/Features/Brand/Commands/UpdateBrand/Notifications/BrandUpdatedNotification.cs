using Application.Abstraction.Messaging;

namespace Application.Features.Brand.Commands.UpdateBrand.Notifications;

public record BrandUpdatedNotification(Guid id) : INotification;
