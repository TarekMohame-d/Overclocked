using Application.Abstraction.Messaging;

namespace Application.Features.Tag.Commands.UpdateTag.Notifications;

public record TagUpdatedNotification(Guid Id) : INotification;
