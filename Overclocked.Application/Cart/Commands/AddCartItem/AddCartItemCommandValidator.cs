using FluentValidation;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Cart.Commands.AddCartItem;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    private readonly IProductRepository _productRepository;
    public AddCartItemCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than 0.");

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
