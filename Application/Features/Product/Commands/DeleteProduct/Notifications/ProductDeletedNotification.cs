using Application.Abstraction.Messaging;

namespace Application.Features.Product.Commands.DeleteProduct.Notifications;

public record ProductDeletedNotification(Guid Id) : INotification;
