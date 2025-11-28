using Application.Services.ReviewReply.DTOs.Request;
using FluentValidation;

namespace Application.Services.ReviewReply.Validations;

public class CreateReviewReplyRequestValidator : AbstractValidator<CreateReviewReplyRequestBody>
{
    public CreateReviewReplyRequestValidator()
    {
        RuleFor(x => x.Reply)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
    }
}
