using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Commands.UpdateReview;
using Overclocked.Unit.Tests.Validations.Review.TestCases;
using Shouldly;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Unit.Tests.Validations.Review;

public class UpdateReviewCommandValidatorTest
{
    private readonly IProductRepository _productRepositoryMock = Substitute.For<IProductRepository>();

    [Theory]
    [MemberData(nameof(UpdateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task UpdateReviewCommandValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new UpdateReviewCommandValidator(_productRepositoryMock);

        var request = new UpdateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = (int)rating!,
            Comment = "Comment"
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<UpdateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(UpdateReviewValidationTestCases.InvalidCommentCases), MemberType = typeof(UpdateReviewValidationTestCases))]
    public async Task UpdateReviewCommandValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new UpdateReviewCommandValidator(_productRepositoryMock);

        var request = new UpdateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 3,
            Comment = comment!
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<UpdateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewCommandValidator_Should_ReturnError_ProductDoesNotExist()
    {
        // Arrange
        var validator = new UpdateReviewCommandValidator(_productRepositoryMock);

        var request = new UpdateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 3,
            Comment = "Comment"
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<UpdateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ProductId).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
