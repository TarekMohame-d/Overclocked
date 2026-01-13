using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Application.Features.ReviewUseCases.GetPagedReviews;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class GetPagedReviewsRequestHandlerTest
{
    private readonly IReviewReadRepository _reviewReadRepository;
    private readonly GetPagedReviewsRequestHandler _getPagedReviewsRequestHandler;

    public GetPagedReviewsRequestHandlerTest()
    {
        _reviewReadRepository = Substitute.For<IReviewReadRepository>();
        _getPagedReviewsRequestHandler = new GetPagedReviewsRequestHandler(_reviewReadRepository);
    }

    [Fact]
    public async Task GetPagedReviewsRequestHandler_Should_ReturnEmptyList_When_NoReviewsFoundForProductId()
    {
        // Arrange
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "asc",
            ProductId = Guid.NewGuid(),
        };

        _reviewReadRepository.CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(0);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _getPagedReviewsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldBeEmpty();
        result.Value.HasNextPage.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);

        await _reviewReadRepository.Received(1).CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedReviewsRequestHandler_Should_ReturnReviews_When_ReviewsFoundForProductId()
    {
        // Arrange
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "asc",
            ProductId = Guid.NewGuid(),
        };

        List<Review> reviews = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);

        _reviewReadRepository.CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(5);

        _reviewReadRepository
            .GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<ReviewSortField>(),
                Arg.Any<Application.Common.Enums.SortDirection>()
            )
            .Returns(reviews);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _getPagedReviewsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.Value.Items.ShouldNotBeEmpty();

        await _reviewReadRepository.Received(1).CountAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewReadRepository
            .Received(1)
            .GetPagedAsync(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<ReviewSortField>(),
                Arg.Any<Application.Common.Enums.SortDirection>()
            );
    }
}
