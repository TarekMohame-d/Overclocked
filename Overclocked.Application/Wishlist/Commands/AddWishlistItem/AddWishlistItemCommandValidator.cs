using FluentValidation;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Wishlist.Commands.AddWishlistItem;

public class AddWishlistItemCommandValidator : AbstractValidator<AddWishlistItemCommand>
{
    private readonly IProductRepository _productRepository;
    public AddWishlistItemCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

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
