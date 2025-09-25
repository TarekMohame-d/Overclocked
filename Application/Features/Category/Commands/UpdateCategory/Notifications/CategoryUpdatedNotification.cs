using Application.Abstraction.Messaging;

namespace Application.Features.Category.Commands.UpdateCategory.Notifications;

public record CategoryUpdatedNotification(Guid id) : INotification;
