// using FluentValidation.TestHelper;
// using NSubstitute;
// using Overclocked.Application.Abstractions.Persistence;
// using Overclocked.Application.Cart.Commands.AddCartItem;
// using Overclocked.Domain.ProductAggregate.ValueObjects;
// using Overclocked.Unit.Tests.Validations.Cart.TestCases;
// using Shouldly;

// namespace Overclocked.Unit.Tests.Validations.Cart;

// public class AddCartItemCommandValidatorTest
// {
//     [Theory]
//     [MemberData(nameof(AddCartItemValidationTestCases.InvalidQuantityCases), MemberType = typeof(AddCartItemValidationTestCases))]
//     public async Task AddCartItemCommandValidator_Should_ReturnError_When_QuantityIsInvalid(int? quantity)
//     {
//         // Arrange
//         IProductRepository productRepositoryMock = Substitute.For<IProductRepository>();
//         var validator = new AddCartItemCommandValidator(productRepositoryMock);

//         var command = new AddCartItemCommand
//         {
//             ProductId = Guid.NewGuid(),
//             Quantity = (int)quantity!,
//             UserId = Guid.NewGuid()
//         };

//         productRepositoryMock
//             .ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
//             .Returns(true);

//         // Act
//         TestValidationResult<AddCartItemCommand> result = await validator.TestValidateAsync(command);

//         // Assert
//         result.IsValid.ShouldBeFalse();
//         result.Errors.ShouldNotBeEmpty();
//         result.ShouldHaveValidationErrorFor(x => x.Quantity).Only();
//     }

//     [Fact]
//     public async Task AddCartItemCommandValidator_Should_ReturnError_When_ProductDoesNotExist()
//     {
//         // Arrange
//         IProductRepository productRepositoryMock = Substitute.For<IProductRepository>();
//         var validator = new AddCartItemCommandValidator(productRepositoryMock);

//         var command = new AddCartItemCommand
//         {
//             ProductId = Guid.NewGuid(),
//             Quantity = 2,
//             UserId = Guid.NewGuid()
//         };

//         productRepositoryMock
//             .ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
//             .Returns(false);

//         // Act
//         TestValidationResult<AddCartItemCommand> result = await validator.TestValidateAsync(command);

//         // Assert
//         result.IsValid.ShouldBeFalse();
//         result.Errors.ShouldNotBeEmpty();
//         result.ShouldHaveValidationErrorFor(x => x.ProductId).Only();

//         await productRepositoryMock.Received()
//             .ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
//     }
// }
