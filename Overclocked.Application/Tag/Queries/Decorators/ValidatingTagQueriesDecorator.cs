using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries.Decorators;

public class ValidatingTagQueriesDecorator(ITagQueries inner,
        IValidator<GetPagedTagsQuery> getPagedValidator) : ITagQueries
{
    public async Task<Result<PagedResult<TagListResponse>>> GetPagedTagsQueryHandler(GetPagedTagsQuery query, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await getPagedValidator.ValidateAsync(query, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult
                .Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<PagedResult<TagListResponse>>.ValidationError<GetPagedTagsQuery>(errorDictionary);
        }

        Result<PagedResult<TagListResponse>> result = await inner.GetPagedTagsQueryHandler(query, cancellationToken);

        return result;
    }
}
