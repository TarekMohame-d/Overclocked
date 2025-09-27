using Application.Abstraction.Messaging;

namespace Application.Features.Tag.Commands.DeleteTag.Notifications;

public record TagDeletedNotification(Guid Id) : INotification;
