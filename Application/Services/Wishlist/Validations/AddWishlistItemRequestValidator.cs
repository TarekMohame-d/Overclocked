using Application.Services.Wishlist.DTOs.Request;
using FluentValidation;

namespace Application.Services.Wishlist.Validations;

public class AddWishlistItemRequestValidator : AbstractValidator<AddWishlistItemRequest>
{
    public AddWishlistItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.");
    }
}
