using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Enums;
using Application.Common.Results;
using Application.Services.Review;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using MockQueryable;
using NSubstitute;
using Shouldly;
using SortDirection = Application.Common.Enums.SortDirection;

namespace Unit.Tests.ReviewTests;

public class GetPagedReviewsAsyncTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewService _reviewService;

    public GetPagedReviewsAsyncTest()
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
    public async Task GetPagedReviewsAsync_Should_ReturnReviews_When_ReviewsExist()
    {
        // Arrange
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            ProductId = Guid.NewGuid()
        };

        List<Review> reviews = new ReviewFaker().Generate(3);
        List<User> users = new UserFaker().Generate(3);

        foreach(Review review in reviews)
        {
            review.User = users[reviews.IndexOf(review)];
        }

        IQueryable<Review> mockQueryable = reviews.BuildMock();

        _reviewRepositoryMock.GetReviewsQuery(Arg.Any<Guid>(), Arg.Any<ReviewSortField>(), Arg.Any<SortDirection>())
            .Returns(mockQueryable);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _reviewService
            .GetPagedReviewsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        reviews.Count.ShouldBe(result.Data.Items.Count);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _reviewRepositoryMock.Received(1)
            .GetReviewsQuery(Arg.Any<Guid>(), Arg.Any<ReviewSortField>(), Arg.Any<SortDirection>());
    }

    [Fact]
    public async Task GetPagedReviewsAsync_Should_ReturnEmptyList_When_ReviewsDoesNotExist()
    {
        // Arrange
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            ProductId = Guid.NewGuid()
        };

        List<Review> reviews = [];

        IQueryable<Review> mockQueryable = reviews.BuildMock();

        _reviewRepositoryMock.GetReviewsQuery(Arg.Any<Guid>(), Arg.Any<ReviewSortField>(), Arg.Any<SortDirection>())
            .Returns(mockQueryable);

        // Act
        Result<PagedResult<ReviewResponse>> result = await _reviewService
            .GetPagedReviewsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _reviewRepositoryMock.Received(1)
            .GetReviewsQuery(Arg.Any<Guid>(), Arg.Any<ReviewSortField>(), Arg.Any<SortDirection>());
    }
}
