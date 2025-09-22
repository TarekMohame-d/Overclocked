using Application.Abstraction.Messaging;

namespace Application.Features.Brand.Commands.DeleteBrand.Notifications;

public record BrandDeletedNotification(Guid Id, string brandImage) : INotification;
