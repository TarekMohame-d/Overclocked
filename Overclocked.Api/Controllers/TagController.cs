using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Application.Tag.Queries.GetPagedTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Api.Controllers;

[ApiController]
public class TagController(
    ICommandHandler<CreateTagCommand> createHandler,
    ICommandHandler<UpdateTagCommand> updateHandler,
    ICommandHandler<DeleteTagCommand> deleteHandler,
    IQueryHandler<GetPagedTagsQuery, PagedResult<TagPagedResponse>> getPagedHandler) : ControllerBase
{
    [HttpGet]
    [Route(TagRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] GetPagedTagsRequest request,
        CancellationToken cancellationToken)
    {
        var query = GetPagedTagsQuery.ToQuery(request);

        Result<PagedResult<TagPagedResponse>> result = await getPagedHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand
        {
            Name = request.Name,
        };

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(TagRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand
        {
            Id = id,
            Name = request.Name
        };

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand
        {
            Id = id
        };

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
