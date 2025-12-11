using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries;

public interface ITagQueries
{
    Task<Result<PagedResult<TagPagedResponse>>> GetPagedTagsQueryHandler(
        GetPagedTagsQuery query,
        CancellationToken cancellationToken);
}
