using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Commands.CreateReview;
using Overclocked.Unit.Tests.Validations.Review.TestCases;
using Shouldly;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Unit.Tests.Validations.Review;

public class CreateReviewCommandValidatorTest
{
    private readonly IProductRepository _productRepositoryMock = Substitute.For<IProductRepository>();

    [Theory]
    [MemberData(nameof(CreateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task CreateReviewCommandValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new CreateReviewCommandValidator(_productRepositoryMock);

        var request = new CreateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = (int)rating!,
            Comment = "Comment"
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(CreateReviewValidationTestCases.InvalidCommentCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task CreateReviewCommandValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new CreateReviewCommandValidator(_productRepositoryMock);

        var request = new CreateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 4,
            Comment = comment!
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewCommandValidator_Should_ReturnError_WhenProductDoesNotExist()
    {
        // Arrange
        var validator = new CreateReviewCommandValidator(_productRepositoryMock);

        var request = new CreateReviewCommand
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 4,
            Comment = "Comment"
        };

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateReviewCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ProductId).Only();

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
