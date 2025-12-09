using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Application.Tag.Commands.DeleteTag;

public record DeleteTagCommand(TagId TagId);
