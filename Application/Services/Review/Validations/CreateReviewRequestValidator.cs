using Application.Services.Review.DTOs.Request;
using FluentValidation;

namespace Application.Services.Review.Validations;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequestBody>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .InclusiveBetween(1, 5)
            .WithMessage("{PropertyName} must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(500)
            .WithMessage("{PropertyName} must not exceed 500 characters.");
    }
}
