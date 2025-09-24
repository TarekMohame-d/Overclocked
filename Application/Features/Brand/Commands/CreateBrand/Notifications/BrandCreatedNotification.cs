using Application.Abstraction.Messaging;

namespace Application.Features.Brand.Commands.CreateBrand.Notifications;

public record BrandCreatedNotification(Guid id) : INotification;
