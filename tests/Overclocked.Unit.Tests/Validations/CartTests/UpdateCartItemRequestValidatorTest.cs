using FluentValidation.TestHelper;
using Overclocked.Application.Features.CartUseCases.UpdateCartItem;
using Overclocked.Unit.Tests.Validations.CartTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.CartTests;

public class UpdateCartItemRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateCartItemValidationTestCases.InvalidQuantityCases),
        MemberType = typeof(UpdateCartItemValidationTestCases)
    )]
    public async Task UpdateCartItemRequestValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
    {
        // Arrange
        var validator = new UpdateCartItemRequestValidator();
        var request = new UpdateCartItemRequest
        {
            UserId = Guid.NewGuid(),
            CartItemId = Guid.NewGuid(),
            Quantity = (int)quantity!,
        };

        // Act
        TestValidationResult<UpdateCartItemRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
    }
}
