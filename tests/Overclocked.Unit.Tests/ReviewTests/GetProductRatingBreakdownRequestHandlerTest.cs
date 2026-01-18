using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Application.Features.ReviewUseCases.GetProductRatingBreakdown;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class GetProductRatingBreakdownRequestHandlerTest
{
    private readonly IReviewReadRepository _reviewReadRepository;
    private readonly GetProductRatingBreakdownRequestHandler _getProductRatingBreakdownRequestHandler;

    public GetProductRatingBreakdownRequestHandlerTest()
    {
        _reviewReadRepository = Substitute.For<IReviewReadRepository>();
        _getProductRatingBreakdownRequestHandler = new GetProductRatingBreakdownRequestHandler(_reviewReadRepository);
    }

    [Fact]
    public async Task GetProductRatingBreakdownRequestHandler_Should_ReturnEmptyDictionary_When_NoReviewsFoundForProductId()
    {
        // Arrange
        var request = new GetProductRatingBreakdownRequest { ProductId = Guid.NewGuid() };

        var ratingBreakdown = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
        };

        _reviewReadRepository
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(ratingBreakdown);

        // Act
        Result<RatingBreakdownResponse> result = await _getProductRatingBreakdownRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEquivalentTo(new RatingBreakdownResponse { Ratings = ratingBreakdown });
        result.Error.ShouldBe(Error.None);

        await _reviewReadRepository
            .Received(1)
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }
}
