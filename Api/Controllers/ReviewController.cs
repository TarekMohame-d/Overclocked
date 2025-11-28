using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ReviewController(IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    [Route(ReviewRoutes.GetReviewsRatingBreakdown)]
    public async Task<IActionResult> GetReviewsRatingBreakdown(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        Result<RatingBreakdownResponse> response = await reviewService
            .GetReviewRatingBreakdownAsync(productId, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationActionAttribute<GetPagedReviewsQuery>))]
    [Route(ReviewRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromRoute] Guid productId,
        [FromQuery] GetPagedReviewsQuery query,
        CancellationToken cancellationToken)
    {
        var request = GetPagedReviewsRequest.FromQuery(query, productId);
        Result<PagedResult<ReviewResponse>> response = await reviewService
            .GetPagedReviewsAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateReviewRequestBody>))]
    [Route(ReviewRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid productId,
        CreateReviewRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var request = CreateReviewRequest.FromBody(requestBody, (Guid)userId, productId);

        Result<ReviewCreatedResponse> response = await reviewService.CreateReviewAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateReviewRequestBody>))]
    [Route(ReviewRoutes.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid productId,
        [FromRoute] Guid id,
        UpdateReviewRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var request = UpdateReviewRequest.FromBody(requestBody, (Guid)userId, productId, id);

        Result<ReviewUpdatedResponse> response = await reviewService.UpdateReviewAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(ReviewRoutes.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid productId,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var request = new DeleteReviewRequest
        {
            ProductId = productId,
            ReviewId = id,
            UserId = (Guid)userId
        };

        Result response = await reviewService.DeleteReviewAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
