using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Commands;

public interface ITagCommands
{
    Task<Result> CreateTagCommandHandler(CreateTagCommand command, CancellationToken cancellationToken);
    Task<Result> UpdateTagCommandHandler(UpdateTagCommand command, CancellationToken cancellationToken);
    Task<Result> DeleteTagCommandHandler(DeleteTagCommand command, CancellationToken cancellationToken);
}
