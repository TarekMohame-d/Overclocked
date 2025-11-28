using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Cart.TestCases;

namespace Unit.Tests.Validations.Cart;

public class UpdateCartItemRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateCartItemValidationTestCases.InvalidQuantityCases), MemberType = typeof(UpdateCartItemValidationTestCases))]
    public async Task UpdateCartItemRequestValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
    {
        // Arrange
        var validator = new UpdateCartItemRequestValidator();
        var request = new UpdateCartItemRequestBody
        {
            Quantity = (int)quantity!
        };

        // Act
        TestValidationResult<UpdateCartItemRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
    }
}
