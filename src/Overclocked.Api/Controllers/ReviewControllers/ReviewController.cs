using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.ReviewUseCases.CreateReview;
using Overclocked.Application.Features.ReviewUseCases.DeleteReview;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Requests;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Application.Features.ReviewUseCases.GetPagedReviews;
using Overclocked.Application.Features.ReviewUseCases.GetProductRatingBreakdown;
using Overclocked.Application.Features.ReviewUseCases.UpdateReview;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers.ReviewControllers;

[ApiController]
public class ReviewController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route(ReviewRoutes.GetReviewsRatingBreakdown)]
    public async Task<IActionResult> GetReviewsRatingBreakdown([FromRoute] Guid productId, CancellationToken ct)
    {
        var request = new GetProductRatingBreakdownRequest { ProductId = productId };

        Result<RatingBreakdownResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(ReviewRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged(
        [FromRoute] Guid productId,
        [FromQuery] GetPagedReviewsQuery query,
        CancellationToken ct
    )
    {
        var request = new GetPagedReviewsRequest
        {
            ProductId = productId,
            Page = query.Page ?? 1,
            PageSize = query.PageSize ?? 10,
            SortBy = query.SortBy ?? string.Empty,
            Direction = query.Direction ?? string.Empty,
        };

        Result<PagedResult<ReviewResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(ReviewRoutes.Create)]
    public async Task<IActionResult> Create([FromRoute] Guid productId, CreateReviewRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new CreateReviewRequest
        {
            UserId = userId.Value,
            ProductId = productId,
            Comment = dto.Comment,
            Rating = dto.Rating,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Created, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPut]
    [Route(ReviewRoutes.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid productId,
        [FromRoute] Guid id,
        CreateReviewRequestDto dto,
        CancellationToken ct
    )
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new UpdateReviewRequest
        {
            ProductId = productId,
            ReviewId = id,
            UserId = userId.Value,
            Comment = dto.Comment,
            Rating = dto.Rating,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(ReviewRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid productId, [FromRoute] Guid id, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new DeleteReviewRequest
        {
            ProductId = productId,
            ReviewId = id,
            UserId = userId.Value,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
