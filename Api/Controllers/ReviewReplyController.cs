using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.ReviewReply.DTOs.Request;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ReviewReplyController(IReviewReplyService reviewReplyService) : ControllerBase
{
    [Authorize(Policy = nameof(PermissionType.ReplyToReview))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateReviewReplyRequestBody>))]
    [Route(ReviewReplyRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        CreateReviewReplyRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? employeeId = HttpContext.GetUserId();
        if(employeeId == null)
        {
            return Unauthorized();
        }

        var request = CreateReviewReplyRequest.FromBody(requestBody, (Guid)employeeId, reviewId, productId);

        Result response = await reviewReplyService.CreateReviewReplyAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.ReplyToReview))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateReviewReplyRequestBody>))]
    [Route(ReviewReplyRoutes.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromRoute] Guid replyId,
        UpdateReviewReplyRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? employeeId = HttpContext.GetUserId();
        if(employeeId == null)
        {
            return Unauthorized();
        }

        var request = UpdateReviewReplyRequest.FromBody(requestBody, (Guid)employeeId, reviewId, productId, replyId);

        Result response = await reviewReplyService.UpdateReviewReplyAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.ReplyToReview))]
    [HttpDelete]
    [Route(ReviewReplyRoutes.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromRoute] Guid replyId,
        CancellationToken cancellationToken)
    {
        var deleteReviewReplyRequest = new DeleteReviewReplyRequest
        {
            ProductId = productId,
            ReviewId = reviewId,
            ReplyId = replyId
        };

        Result response = await reviewReplyService.DeleteReviewReplyAsync(deleteReviewReplyRequest, cancellationToken);

        return response.ToActionResult();
    }
}
