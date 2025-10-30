using Api.ActionFilters;
using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Services;
using Application.Services.Category.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryServices;
    public CategoryController(ICategoryService categoryServices)
    {
        _categoryServices = categoryServices;
    }

    [HttpGet]
    [Route(CategoryRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetCategoryByIdRequest { Id = id };
        var response = await _categoryServices.GetCategoryByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(CategoryRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var request = new GetAllCategoriesRequest();
        var response = await _categoryServices.GetAllCategoriesAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateCategoryRequest>))]
    [Route(CategoryRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var response = await _categoryServices.CreateCategoryAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateCategoryRequest>))]
    [Route(CategoryRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };

        var response = await _categoryServices.UpdateCategoryAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(CategoryRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteCategoryRequest { Id = id };
        var response = await _categoryServices.DeleteCategoryAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
