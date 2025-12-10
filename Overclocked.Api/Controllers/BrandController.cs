using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Brand.Commands;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Application.Brand.Queries;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class BrandController(IBrandQueries brandQueries, IBrandCommands brandCommands) : ControllerBase
{
    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBrandQuery
        {
            Id = BrandId.Create(id)
        };

        Result<BrandResponse> response = await brandQueries.GetBrandQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetBrandListQuery();

        Result<IEnumerable<BrandListResponse>> response = await brandQueries
            .GetBrandListQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [Route(BrandRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBrandCommand
        {
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        Result response = await brandCommands.CreateBrandCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [Route(BrandRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBrandCommand
        {
            Id = id,
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        Result response = await brandCommands.UpdateBrandCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(BrandRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBrandCommand
        {
            Id = id
        };

        Result response = await brandCommands.DeleteBrandCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
