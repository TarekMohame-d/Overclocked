using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Wishlist.Commands.AddWishlistItem;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Wishlist;

public class AddWishlistItemCommandValidatorTest
{
    [Fact]
    public async Task AddCartItemCommandValidator_Should_ReturnError_When_ProductDoesNotExist()
    {
        // Arrange
        IProductRepository productRepositoryMock = Substitute.For<IProductRepository>();
        var validator = new AddWishlistItemCommandValidator(productRepositoryMock);

        var command = new AddWishlistItemCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        productRepositoryMock.AnyAsync(
            Arg.Any<Expression<Func<Domain.ProductAggregate.Product, bool>>>(),
            Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<AddWishlistItemCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ProductId).Only();

        await productRepositoryMock.Received()
            .AnyAsync(
            Arg.Any<Expression<Func<Domain.ProductAggregate.Product, bool>>>(),
            Arg.Any<CancellationToken>());
    }
}
