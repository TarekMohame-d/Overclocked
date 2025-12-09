using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Category.Commands;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Application.Category.Queries;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Application.Category.Queries.GetCategory;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CategoryController(ICategoryQueries categoryQueries, ICategoryCommands categoryCommands) : ControllerBase
{
    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryQuery { Id = CategoryId.Create(id) };

        Result<CategoryResponse> response = await categoryQueries.GetCategoryQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetCategoryListQuery();
        Result<IEnumerable<CategoryListResponse>> response = await categoryQueries
            .GetCategoryListQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.ImageUrl);
        Result response = await categoryCommands.CreateCategoryCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(CategoryId.Create(id), request.Name, request.ImageUrl);

        Result response = await categoryCommands.UpdateCategoryCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(CategoryId.Create(id));

        Result response = await categoryCommands.DeleteCategoryCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
