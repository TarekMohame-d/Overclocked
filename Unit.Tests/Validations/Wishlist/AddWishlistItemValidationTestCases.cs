using Application.Services.Wishlist.DTOs.Request;
using Application.Services.Wishlist.Validations;
using FluentValidation.TestHelper;
using Shouldly;

namespace Unit.Tests.Validations.Wishlist;

public class AddWishlistItemValidationTestCases
{
    [Fact]
    public async Task AddWishlistItemRequestValidator_Should_ReturnError_When_ProductIdIsInvalid()
    {
        // Arrange
        var validator = new AddWishlistItemRequestValidator();
        var request = new AddWishlistItemRequest { ProductId = Guid.Empty };

        // Act
        TestValidationResult<AddWishlistItemRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ProductId).Only();
    }
}
