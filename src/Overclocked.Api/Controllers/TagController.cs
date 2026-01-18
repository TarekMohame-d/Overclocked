using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.TagUseCases.CreateTag;
using Overclocked.Application.Features.TagUseCases.DeleteTag;
using Overclocked.Application.Features.TagUseCases.DTOs.Requests;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.GetPagedTags;
using Overclocked.Application.Features.TagUseCases.UpdateTag;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class TagController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route(TagRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedTagsQuery query, CancellationToken ct)
    {
        var request = new GetPagedTagsRequest
        {
            Page = query.Page ?? 1,
            PageSize = query.PageSize ?? 10,
            SearchTerm = query.SearchTerm ?? string.Empty,
            SortBy = query.SortBy ?? string.Empty,
            Direction = query.Direction ?? string.Empty,
        };

        Result<PagedResult<TagPagedResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequestDto dto, CancellationToken ct)
    {
        var request = new CreateTagRequest { Name = dto.Name };

        Result<Guid> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: x => Created(string.Empty, x), onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(TagRoutes.Update)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateTagRequestDto dto, CancellationToken ct)
    {
        var request = new UpdateTagRequest { Id = id, Name = dto.Name };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteTagRequest { Id = id };

        Result result = await dispatcher.Send(request, cancellationToken);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
