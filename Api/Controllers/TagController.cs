using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class TagController(ITagService tagService) : ControllerBase
{
    [HttpGet]
    [Route(TagRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetTagByIdRequest { Id = id };
        Result<TagResponse> response = await tagService.GetTagByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationActionAttribute<GetPagedTagsQuery>))]
    [Route(TagRoutes.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetPagedTagsQuery query, CancellationToken cancellationToken)
    {
        var request = GetPagedTagsRequest.FromQuery(query);
        Result<PagedResult<TagListResponse>> response = await tagService.GetPagedTagsAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateTagRequest>))]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        Result response = await tagService.CreateTagAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateTagRequest>))]
    [Route(TagRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateTagRequestBody request,
        CancellationToken cancellationToken
    )
    {
        UpdateTagRequest updateTagRequest = new() { Id = id, Name = request.Name };

        Result response = await tagService.UpdateTagAsync(updateTagRequest, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteTagRequest { Id = id };
        Result response = await tagService.DeleteTagAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
