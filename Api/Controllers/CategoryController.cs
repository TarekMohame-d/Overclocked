using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Messaging;
using Application.Features.Category.Queries.GetAllCategories;
using Application.Features.Category.Queries.GetCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery();
        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    // [Authorize]
    // [HttpPost]
    // [Route(CategoryRoutes.Create)]
    // public async Task<IActionResult> Create([FromForm] CreateCategoryCommand command, CancellationToken cancellationToken)
    // {
    //     var response = await _sender.Send(command, cancellationToken);

    //     return ResponseHandler(response);
    // }

    // [Authorize]
    // [HttpPut]
    // [Route(CategoryRoutes.Update)]
    // public async Task<IActionResult> Put([FromRoute] Guid id, [FromForm] UpdateCategoryCommand request, CancellationToken cancellationToken)
    // {
    //     var command = new UpdateCategoryWithIdCommand
    //     {
    //         Id = id,
    //         Name = request.Name,
    //         ImageFile = request.ImageFile,
    //         ImageUrl = request.ImageUrl
    //     };

    //     var response = await _sender.Send(command, cancellationToken);

    //     return ResponseHandler(response);
    // }

    // [Authorize]
    // [HttpDelete]
    // [Route(CategoryRoutes.Delete)]
    // public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    // {
    //     var command = new DeleteCategoryCommand { Id = id };
    //     var response = await _sender.Send(command, cancellationToken);

    //     return ResponseHandler(response);
    // }
}
