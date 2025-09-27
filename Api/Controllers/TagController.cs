using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Messaging;
using Application.Features.Tag.Commands.CreateTag;
using Application.Features.Tag.Commands.DeleteTag;
using Application.Features.Tag.Commands.UpdateTag;
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

    //[Authorize]
    [HttpPost]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [Route(TagRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateTagCommand request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagWithIdCommand
        {
            Id = id,
            Name = request.Name
        };

        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }
}
