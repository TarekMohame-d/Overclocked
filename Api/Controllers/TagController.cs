using Api.ActionFilters;
using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Services;
using Application.Services.Tag.DTOs.Request;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers;

[ApiController]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [Route(TagRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetTagByIdRequest { Id = id };
        var response = await _tagService.GetTagByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationActionAttribute<GetPagedTagsRequest>))]
    [Route(TagRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPagedTagsRequest request,
        CancellationToken cancellationToken)
    {
        var query = GetPagedTagsQuery.FromRequest(request);
        var response = await _tagService.GetPagedTagsAsync(query, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateTagRequest>))]
    [Route(TagRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var response = await _tagService.CreateTagAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateTagRequest>))]
    [Route(TagRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };
        var response = await _tagService.UpdateTagAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(TagRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteTagRequest { Id = id };
        var response = await _tagService.DeleteTagAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
