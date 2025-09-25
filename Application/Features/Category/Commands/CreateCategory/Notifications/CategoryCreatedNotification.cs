using Application.Abstraction.Messaging;

namespace Application.Features.Category.Commands.CreateCategory.Notifications;

public record CategoryCreatedNotification(Guid id) : INotification;
