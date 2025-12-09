using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Application.Tag.Commands.UpdateTag;

public record UpdateTagCommand(TagId TagId, string Name);
