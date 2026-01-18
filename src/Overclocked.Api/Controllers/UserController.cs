using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.UserUseCases.AddAddress;
using Overclocked.Application.Features.UserUseCases.DeleteAddress;
using Overclocked.Application.Features.UserUseCases.DTOs.Requests;
using Overclocked.Application.Features.UserUseCases.DTOs.Responses;
using Overclocked.Application.Features.UserUseCases.GetAddresses;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class UserController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Customer))]
    [HttpGet]
    [Route(UserRoutes.GetAllAddresses)]
    public async Task<IActionResult> GetAddresses(CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new GetAddressesRequest { UserId = userId.Value };

        Result<IEnumerable<AddressResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(Ok, error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(UserRoutes.AddAddress)]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new AddAddressRequest
        {
            UserId = userId.Value,
            Apartment = dto.Apartment,
            Building = dto.Building,
            Street = dto.Street,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Description = dto.Description,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(Created, error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(UserRoutes.DeleteAddress)]
    public async Task<IActionResult> DeleteAddress([FromBody] DeleteAddressRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new DeleteAddressRequest
        {
            UserId = userId.Value,
            Apartment = dto.Apartment,
            Building = dto.Building,
            Street = dto.Street,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Description = dto.Description,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(NoContent, error => error.ToProblemDetails(this));
    }
}
