using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CategoryController(ICategoryService categoryServices) : ControllerBase
{
    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetCategoryByIdRequest { Id = id };
        Result<CategoryResponse> response = await categoryServices.GetCategoryByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var request = new GetAllCategoriesRequest();
        Result<IEnumerable<CategoryListResponse>> response = await categoryServices
            .GetAllCategoriesAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateCategoryRequest>))]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await categoryServices.CreateCategoryAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateCategoryRequestBody>))]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        UpdateCategoryRequest request = new()
        {
            Id = id,
            Name = requestBody.Name,
            ImageUrl = requestBody.ImageUrl,
        };

        Result response = await categoryServices.UpdateCategoryAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Result response = await categoryServices.DeleteCategoryAsync(id, cancellationToken);

        return response.ToActionResult();
    }
}
