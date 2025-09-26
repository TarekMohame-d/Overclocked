using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Messaging;
using Application.Features.Tag.Queries.GetPagedTags;
using Application.Features.Tag.Queries.GetTagById;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;

[ApiController]
public class TagController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route(TagRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTagByIdQuery { Id = id };
        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(TagRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPagedTagsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedTagsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };

        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }
}
