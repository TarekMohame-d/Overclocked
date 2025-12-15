using FluentValidation;

namespace Overclocked.Application.ReviewReply.Commands.UpdateReviewReply;

public class UpdateReviewReplyCommandValidator : AbstractValidator<UpdateReviewReplyCommand>
{
    public UpdateReviewReplyCommandValidator()
    {
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
    }
}
