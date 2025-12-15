using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Queries.GetProductRatingBreakdown;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class GetProductRatingBreakdownQueryHandlerTest
{
    private readonly IReviewRepository _reviewRepository;
    private readonly GetProductRatingBreakdownQueryHandler _getProductRatingBreakdownQueryHandler;

    public GetProductRatingBreakdownQueryHandlerTest()
    {
        _reviewRepository = Substitute.For<IReviewRepository>();
        _getProductRatingBreakdownQueryHandler = new GetProductRatingBreakdownQueryHandler(_reviewRepository);
    }

    [Fact]
    public async Task GetProductRatingBreakdownQueryHandler_Should_ReturnEmptyDictionary_When_NoReviewsFoundForProductId()
    {
        // Arrange
        var query = new GetProductRatingBreakdownQuery
        {
            ProductId = Guid.NewGuid()
        };

        var ratingBreakdown = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

        _reviewRepository.GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(ratingBreakdown);

        // Act
        Result<RatingBreakdownResponse> result = await _getProductRatingBreakdownQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(new RatingBreakdownResponse { Ratings = ratingBreakdown });
        result.Error.ShouldBe(Error.None);

        await _reviewRepository.Received(1)
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }
}
