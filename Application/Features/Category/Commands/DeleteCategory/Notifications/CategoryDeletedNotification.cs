using Application.Abstraction.Messaging;

namespace Application.Features.Category.Commands.DeleteCategory.Notifications;

public record CategoryDeletedNotification(Guid Id, string ImageUrl) : INotification;
