using Application.Abstraction.Messaging;

namespace Application.Features.Product.Commands.UpdateProduct.Notifications;

public record ProductUpdatedNotification(Guid Id) : INotification;
