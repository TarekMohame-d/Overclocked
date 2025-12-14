using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Application.Brand.Commands.DeleteBrand;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Brand.Queries.GetBrandById;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class BrandController(
    ICommandHandler<CreateBrandCommand> createHandler,
    ICommandHandler<UpdateBrandCommand> updateHandler,
    ICommandHandler<DeleteBrandCommand> deleteHandler,
    IQueryHandler<GetBrandByIdQuery, BrandResponse> getByIdHandler,
    IQueryHandler<GetAllBrandsQuery, IEnumerable<BrandListResponse>> getAllHandler) : ControllerBase
{
    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBrandByIdQuery
        {
            Id = BrandId.Create(id)
        };

        Result<BrandResponse> result = await getByIdHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllBrandsQuery();

        Result<IEnumerable<BrandListResponse>> result = await getAllHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
