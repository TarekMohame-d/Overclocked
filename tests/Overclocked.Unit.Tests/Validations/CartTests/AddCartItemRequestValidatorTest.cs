using FluentValidation.TestHelper;
using Overclocked.Application.Features.CartUseCases.AddCartItem;
using Overclocked.Unit.Tests.Validations.CartTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.CartTests;

public class AddCartItemRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(AddCartItemValidationTestCases.InvalidQuantityCases), MemberType = typeof(AddCartItemValidationTestCases))]
    public async Task AddCartItemRequestValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
    {
        // Arrange
        var validator = new AddCartItemRequestValidator();

        var request = new AddCartItemRequest
        {
            ProductId = Guid.NewGuid(),
            Quantity = (int)quantity!,
            UserId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<AddCartItemRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
    }
}
