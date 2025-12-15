using FluentValidation;

namespace Overclocked.Application.ReviewReply.Commands.CreateReviewReply;

public class CreateReviewReplyCommandValidator : AbstractValidator<CreateReviewReplyCommand>
{
    public CreateReviewReplyCommandValidator()
    {
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
    }
}
