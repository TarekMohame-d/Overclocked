using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Review.Queries.GetPagedReviews;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class GetPagedReviewsQueryHandlerTest
{
    private readonly IReviewRepository _reviewRepository;
    private readonly GetPagedReviewsQueryHandler _getPagedReviewsQueryHandler;

    public GetPagedReviewsQueryHandlerTest()
    {
        _reviewRepository = Substitute.For<IReviewRepository>();
        _getPagedReviewsQueryHandler = new GetPagedReviewsQueryHandler(_reviewRepository);
    }

    [Fact]
    public async Task GetPagedReviewsQueryHandler_Should_ReturnEmptyList_When_NoReviewsFoundForProductId()
    {
        // Arrange
        var query = new GetPagedReviewsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "asc",
            ProductId = Guid.NewGuid()
        };

        _reviewRepository.CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(0);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _getPagedReviewsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldBeEmpty();
        result.Value.HasNextPage.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);

        await _reviewRepository.Received(1)
            .CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedReviewsQueryHandler_Should_ReturnReviews_When_ReviewsFoundForProductId()
    {
        // Arrange
        var query = new GetPagedReviewsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "asc",
            ProductId = Guid.NewGuid()
        };

        List<Review> reviews = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);

        _reviewRepository.CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(5);

        _reviewRepository.GetPagedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<ReviewSortField>(),
            Arg.Any<Application.Common.Enums.SortDirection>())
            .Returns(reviews);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _getPagedReviewsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.Value.Items.ShouldNotBeEmpty();

        await _reviewRepository.Received(1)
            .CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepository.Received(1)
            .GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<ReviewSortField>(),
                Arg.Any<Application.Common.Enums.SortDirection>());
    }
}
