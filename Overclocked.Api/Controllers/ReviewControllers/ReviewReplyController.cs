using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.ReviewReply.Commands.CreateReviewReply;
using Overclocked.Application.ReviewReply.Commands.DeleteReviewReply;
using Overclocked.Application.ReviewReply.Commands.UpdateReviewReply;
using Overclocked.Contracts.ReviewReply;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers.ReviewControllers;

[ApiController]
public class ReviewReplyController(
    ICommandHandler<CreateReviewReplyCommand> createHandler,
    ICommandHandler<UpdateReviewReplyCommand> updateHandler,
    ICommandHandler<DeleteReviewReplyCommand> deleteHandler) : ControllerBase
{
    [Authorize(Roles = $"{nameof(RoleType.Admin)}, {nameof(RoleType.SuperAdmin)}")]
    [HttpPost]
    [Route(ReviewReplyRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        CreateReviewReplyRequest request,
        CancellationToken cancellationToken)
    {
        Guid? employeeId = HttpContext.GetUserId();
        if(employeeId == null)
        {
            return Unauthorized();
        }

        var command = new CreateReviewReplyCommand
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            Reply = request.Reply
        };

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)}, {nameof(RoleType.SuperAdmin)}")]
    [HttpPut]
    [Route(ReviewReplyRoutes.Update)]
    public async Task<IActionResult> Update(
    [FromRoute] Guid productId,
    [FromRoute] Guid reviewId,
    [FromRoute] Guid replyId,
    UpdateReviewReplyRequest request,
    CancellationToken cancellationToken)
    {
        Guid? employeeId = HttpContext.GetUserId();
        if(employeeId == null)
        {
            return Unauthorized();
        }

        var command = new UpdateReviewReplyCommand
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            ReplyId = replyId,
            Reply = request.Reply,
        };

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = $"{nameof(RoleType.Admin)}, {nameof(RoleType.SuperAdmin)}")]
    [HttpDelete]
    [Route(ReviewReplyRoutes.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromRoute] Guid replyId,
        CancellationToken cancellationToken)
    {
        Guid? employeeId = HttpContext.GetUserId();
        if(employeeId == null)
        {
            return Unauthorized();
        }

        var command = new DeleteReviewReplyCommand
        {
            EmployeeId = employeeId.Value,
            ProductId = productId,
            ReviewId = reviewId,
            ReplyId = replyId
        };

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
