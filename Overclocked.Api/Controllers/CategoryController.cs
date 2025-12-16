using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Application.Category.Queries.GetCategoryById;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CategoryController(
    ICommandHandler<CreateCategoryCommand> createHandler,
    ICommandHandler<UpdateCategoryCommand> updateHandler,
    ICommandHandler<DeleteCategoryCommand> deleteHandler,
    IQueryHandler<GetCategoryByIdQuery, CategoryResponse> getByIdHandler,
    IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryListResponse>> getAllHandler) : ControllerBase
{
    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery
        {
            Id = CategoryId.Create(id)
        };

        Result<CategoryResponse> result = await getByIdHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery();

        Result<IEnumerable<CategoryListResponse>> result = await getAllHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand
        {
            Id = id,
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand
        {
            Id = id
        };

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
