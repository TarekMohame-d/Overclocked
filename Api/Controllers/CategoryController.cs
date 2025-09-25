using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Messaging;
using Application.Features.Category.Commands.CreateCategory;
using Application.Features.Category.Commands.DeleteCategory;
using Application.Features.Category.Commands.UpdateCategory;
using Application.Features.Category.Queries.GetAllCategories;
using Application.Features.Category.Queries.GetCategoryById;
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
    [HttpPost]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    // [Authorize]
    [HttpPut]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryWithIdCommand
        {
            Id = id,
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    // [Authorize]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }
}
