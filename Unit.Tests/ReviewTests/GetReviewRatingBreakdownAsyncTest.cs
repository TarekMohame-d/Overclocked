using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Review;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ReviewTests;

public class GetReviewRatingBreakdownAsyncTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewService _reviewService;

    public GetReviewRatingBreakdownAsyncTest()
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
    public async Task GetReviewRatingBreakdownAsync_Should_ReturnDefaultRating_When_ProductHasNoReviews()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviewsBreakdown = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

        _reviewRepositoryMock.GetProductRatingsBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(reviewsBreakdown);

        // Act
        Result result = await _reviewService.GetReviewRatingBreakdownAsync(productId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .GetProductRatingsBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetReviewRatingBreakdownAsync_Should_ReturnRating_When_ProductHasReviews()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviewsBreakdown = new Dictionary<int, int> { { 1, 5 }, { 2, 10 }, { 3, 8 }, { 4, 13 }, { 5, 27 } };

        _reviewRepositoryMock.GetProductRatingsBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(reviewsBreakdown);

        // Act
        Result result = await _reviewService.GetReviewRatingBreakdownAsync(productId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .GetProductRatingsBreakdownAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
