using FluentValidation.TestHelper;
using Overclocked.Application.Cart.Commands.UpdateCartItem;
using Overclocked.Unit.Tests.Validations.Cart.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Cart;

public class UpdateCartItemCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateCartItemValidationTestCases.InvalidQuantityCases), MemberType = typeof(UpdateCartItemValidationTestCases))]
    public async Task UpdateCartItemRequestValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
    {
        // Arrange
        var validator = new UpdateCartItemCommandValidator();
        var request = new UpdateCartItemCommand
        {
            UserId = Guid.NewGuid(),
            CartItemId = Guid.NewGuid(),
            Quantity = (int)quantity!
        };

        // Act
        TestValidationResult<UpdateCartItemCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
    }
}
