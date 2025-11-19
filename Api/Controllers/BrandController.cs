using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class BrandController(IBrandService brandServices) : ControllerBase
{
    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetBrandByIdRequest { Id = id };
        Result<BrandResponse> response = await brandServices.GetBrandByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var request = new GetAllBrandsRequest();
        Result<IEnumerable<BrandListResponse>> response = await brandServices.GetAllBrandsAsync(
            request,
            cancellationToken
        );

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateBrandRequest>))]
    [Route(BrandRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken cancellationToken)
    {
        Result response = await brandServices.CreateBrandAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateBrandRequest>))]
    [Route(BrandRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandRequestBody request,
        CancellationToken cancellationToken
    )
    {
        UpdateBrandRequest updateBrandRequest = new()
        {
            Id = id,
            Name = request.Name,
            ImageUrl = request.ImageUrl,
        };

        Result response = await brandServices.UpdateBrandAsync(updateBrandRequest, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(BrandRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteBrandRequest { Id = id };
        Result response = await brandServices.DeleteBrandAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
