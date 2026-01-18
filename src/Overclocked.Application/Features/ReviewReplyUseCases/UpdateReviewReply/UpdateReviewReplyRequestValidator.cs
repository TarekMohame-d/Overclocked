using FluentValidation;

namespace Overclocked.Application.Features.ReviewReplyUseCases.UpdateReviewReply;

public class UpdateReviewReplyRequestValidator : AbstractValidator<UpdateReviewReplyRequest>
{
    public UpdateReviewReplyRequestValidator() =>
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
}
