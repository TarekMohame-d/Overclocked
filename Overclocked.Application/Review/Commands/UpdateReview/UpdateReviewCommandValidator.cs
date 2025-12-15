using FluentValidation;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Review.Commands.UpdateReview;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    private readonly IProductRepository _productRepository;
    public UpdateReviewCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

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

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MustAsync(async (id, cancellation) =>
            {
                return await _productRepository.AnyAsync(x => x.Id == ProductId.Create(id), cancellation);
            })
            .WithMessage("{PropertyName} does not exist.");
    }
}
