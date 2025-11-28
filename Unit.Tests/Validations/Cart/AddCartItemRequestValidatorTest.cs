using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Cart.TestCases;

namespace Unit.Tests.Validations.Cart;

public class AddCartItemRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(AddCartItemValidationTestCases.InvalidQuantityCases), MemberType = typeof(AddCartItemValidationTestCases))]
    public async Task AddCartItemRequestValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
    {
        // Arrange
        var validator = new AddCartItemRequestValidator();
        var request = new AddCartItemRequestBody
        {
            ProductId = Guid.NewGuid(),
            Quantity = (int)quantity!
        };

        // Act
        TestValidationResult<AddCartItemRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
    }
}
