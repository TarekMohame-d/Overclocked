using FluentValidation;

namespace Overclocked.Application.Features.WishlistUseCases.AddWishlistItem;

public class AddWishlistItemRequestValidator : AbstractValidator<AddWishlistItemRequest>
{
    public AddWishlistItemRequestValidator() => RuleFor(x => x.ProductId).NotEmpty().WithMessage("{PropertyName} is required.");
}
