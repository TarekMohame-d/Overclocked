using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.CategoryUseCases.CreateCategory;
using Overclocked.Application.Features.CategoryUseCases.DeleteCategory;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Requests;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.GetAllCategories;
using Overclocked.Application.Features.CategoryUseCases.GetCategoryById;
using Overclocked.Application.Features.CategoryUseCases.UpdateCategory;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CategoryController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetCategoryByIdRequest { Id = id };

        Result<CategoryResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var request = new GetAllCategoriesRequest();

        Result<IEnumerable<CategoryListResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto dto, CancellationToken ct)
    {
        var request = new CreateCategoryRequest { Name = dto.Name, ImageUrl = dto.ImageUrl };

        Result<Guid> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: x => Created(string.Empty, x), onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto dto, CancellationToken ct)
    {
        var request = new UpdateCategoryRequest
        {
            Id = id,
            Name = dto.Name,
            ImageUrl = dto.ImageUrl,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteCategoryRequest { Id = id };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
