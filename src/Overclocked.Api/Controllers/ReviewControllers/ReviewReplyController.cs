using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.ReviewReplyUseCases.CreateReviewReply;
using Overclocked.Application.Features.ReviewReplyUseCases.DeleteReviewReply;
using Overclocked.Application.Features.ReviewReplyUseCases.DTOs.Requests;
using Overclocked.Application.Features.ReviewReplyUseCases.UpdateReviewReply;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers.ReviewControllers;

[ApiController]
public class ReviewReplyController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.SuperAdmin)}")]
    [HttpPost]
    [Route(ReviewReplyRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        CreateReviewReplyRequestDto dto,
        CancellationToken ct
    )
    {
        Guid? employeeId = HttpContext.GetUserId();
        if (employeeId is null)
            return Unauthorized();

        var request = new CreateReviewReplyRequest
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            Reply = dto.Reply,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Created, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.SuperAdmin)}")]
    [HttpPut]
    [Route(ReviewReplyRoutes.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromRoute] Guid replyId,
        UpdateReviewReplyRequestDto dto,
        CancellationToken ct
    )
    {
        Guid? employeeId = HttpContext.GetUserId();
        if (employeeId is null)
            return Unauthorized();

        var request = new UpdateReviewReplyRequest
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            ReplyId = replyId,
            Reply = dto.Reply,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.SuperAdmin)}")]
    [HttpDelete]
    [Route(ReviewReplyRoutes.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromRoute] Guid replyId,
        CancellationToken ct
    )
    {
        Guid? employeeId = HttpContext.GetUserId();
        if (employeeId is null)
            return Unauthorized();

        var request = new DeleteReviewReplyRequest
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            ReplyId = replyId,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
