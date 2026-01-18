using FluentValidation;

namespace Overclocked.Application.Features.ReviewReplyUseCases.CreateReviewReply;

public class CreateReviewReplyRequestValidator : AbstractValidator<CreateReviewReplyRequest>
{
    public CreateReviewReplyRequestValidator() =>
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
}
