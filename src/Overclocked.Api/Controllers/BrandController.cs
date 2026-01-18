using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.BrandUseCases.CreateBrand;
using Overclocked.Application.Features.BrandUseCases.DeleteBrand;
using Overclocked.Application.Features.BrandUseCases.DTOs.Requests;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.BrandUseCases.GetAllBrands;
using Overclocked.Application.Features.BrandUseCases.GetBrandById;
using Overclocked.Application.Features.BrandUseCases.UpdateBrand;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class BrandController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetBrandByIdRequest { Id = id };

        Result<BrandResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        Result<IEnumerable<BrandListResponse>> result = await dispatcher.Send(new GetAllBrandsRequest(), ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(BrandRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequestDto dto, CancellationToken ct)
    {
        var request = new CreateBrandRequest { Name = dto.Name, ImageUrl = dto.ImageUrl };

        Result<Guid> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: x => Created(string.Empty, x), onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(BrandRoutes.Update)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBrandRequestDto dto, CancellationToken ct)
    {
        var request = new UpdateBrandRequest
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
    [Route(BrandRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteBrandRequest { Id = id };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
