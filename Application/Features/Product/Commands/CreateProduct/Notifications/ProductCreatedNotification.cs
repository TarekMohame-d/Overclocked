using Application.Abstraction.Messaging;

namespace Application.Features.Product.Commands.CreateProduct.Notifications;

public record ProductCreatedNotification() : INotification;
