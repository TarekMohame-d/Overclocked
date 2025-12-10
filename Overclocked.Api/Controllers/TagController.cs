using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Tag.Commands;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Application.Tag.Queries;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Api.Controllers;

[ApiController]
public class TagController(ITagQueries tagQueries, ITagCommands tagCommands) : ControllerBase
{
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
            SearchTerm = request.SearchTerm,
            SortBy = request.SortBy,
            Direction = request.Direction
        };

        Result<PagedResult<TagListResponse>> response = await tagQueries
            .GetPagedTagsQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand
        {
            Name = request.Name,
        };

        Result response = await tagCommands.CreateTagCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
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

        Result response = await tagCommands.UpdateTagCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand
        {
            Id = id
        };

        Result response = await tagCommands.DeleteTagCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
