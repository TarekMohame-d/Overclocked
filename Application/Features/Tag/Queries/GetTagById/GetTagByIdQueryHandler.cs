using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using System.Net;

namespace Application.Features.Tag.Queries.GetTagById;

public class GetTagByIdQueryHandler : IQueryHandler<GetTagByIdQuery, Result<TagDto>>
{
    private readonly ITagRepository _tagRepository;

    public GetTagByIdQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<TagDto>> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result<TagDto>.Failure(
                Errors.TagNotFound,
                HttpStatusCode.NotFound);

        return tag.ToDto();
    }
}
