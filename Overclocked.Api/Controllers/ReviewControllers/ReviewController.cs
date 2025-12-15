using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Review.Commands.CreateReview;
using Overclocked.Application.Review.Commands.DeleteReview;
using Overclocked.Application.Review.Commands.UpdateReview;
using Overclocked.Application.Review.Queries.GetPagedReviews;
using Overclocked.Application.Review.Queries.GetProductRatingBreakdown;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers.ReviewControllers;

[ApiController]
public class ReviewController(
    ICommandHandler<CreateReviewCommand> createHandler,
    ICommandHandler<UpdateReviewCommand> updateHandler,
    ICommandHandler<DeleteReviewCommand> deleteHandler,
    IQueryHandler<GetProductRatingBreakdownQuery, RatingBreakdownResponse> getRatingHandler,
    IQueryHandler<GetPagedReviewsQuery, PagedResult<ReviewResponse>> getPagedHandler) : ControllerBase
{
    [HttpGet]
    [Route(ReviewRoutes.GetReviewsRatingBreakdown)]
    public async Task<IActionResult> GetReviewsRatingBreakdown(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductRatingBreakdownQuery
        {
            ProductId = productId
        };

        Result<RatingBreakdownResponse> result = await getRatingHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(ReviewRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged(
        [FromRoute] Guid productId,
        [FromQuery] GetPagedReviewsRequest request,
        CancellationToken cancellationToken)
    {
        var query = GetPagedReviewsQuery.ToQuery(request, productId);

        Result<PagedResult<ReviewResponse>> result = await getPagedHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPost]
    [Route(ReviewRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid productId,
        CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new CreateReviewCommand
        {
            UserId = userId.Value,
            ProductId = productId,
            Comment = request.Comment,
            Rating = request.Rating
        };

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPut]
    [Route(ReviewRoutes.Update)]
    public async Task<IActionResult> Update(
    [FromRoute] Guid productId,
    [FromRoute] Guid id,
    CreateReviewRequest request,
    CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new UpdateReviewCommand
        {
            ProductId = productId,
            ReviewId = id,
            UserId = userId.Value,
            Comment = request.Comment,
            Rating = request.Rating
        };

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
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

        var command = new DeleteReviewCommand
        {
            ProductId = productId,
            ReviewId = id,
            UserId = userId.Value
        };

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
