using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Review;
using Application.Services.Review.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ReviewTests;

public class DeleteReviewAsyncTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewService _reviewService;

    public DeleteReviewAsyncTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cacheServiceMock = Substitute.For<ICacheService>();

        _reviewService = new ReviewService(
            _reviewRepositoryMock,
            _productRepositoryMock,
            _unitOfWorkMock,
            _cacheServiceMock);
    }

    [Fact]
    public async Task DeleteReviewAsync_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid()
        };

        _reviewRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _reviewService.DeleteReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewAsync_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new DeleteReviewRequest
        {
            UserId = userId,
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid()
        };

        Review review = new ReviewFaker().Generate();

        review.UserId = userId;

        _reviewRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _reviewService.DeleteReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewAsync_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new DeleteReviewRequest
        {
            UserId = userId,
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid()
        };

        Review review = new ReviewFaker().Generate();
        Product product = new ProductFaker().Generate();

        review.UserId = userId;
        review.Product = product;

        _reviewRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _cacheServiceMock.RemoveAsync(Arg.Any<string>())
            .Returns(Task.FromResult(true));

        // Act
        Result result = await _reviewService.DeleteReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Review, bool>>>(),
                Arg.Any<Func<IQueryable<Review>, IQueryable<Review>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        _reviewRepositoryMock.Received(1)
            .Delete(Arg.Any<Review>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _cacheServiceMock.Received(1)
            .RemoveAsync(Arg.Any<string>());
    }
}
