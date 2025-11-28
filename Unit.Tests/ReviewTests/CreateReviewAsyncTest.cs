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

public class CreateReviewAsyncTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewService _reviewService;

    public CreateReviewAsyncTest()
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
    public async Task CreateReviewAsync_Should_ReturnFailure_When_ReviewAlreadyExist()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _reviewService.CreateReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _reviewRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewAsync_Should_ReturnFailure_When_ReviewProductNotExist()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _productRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _reviewService.CreateReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewAsync_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        Product product = new ProductFaker().Generate();

        _productRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(product);

        Review review = new ReviewFaker().Generate();

        _reviewRepositoryMock.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _cacheServiceMock.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _reviewService.CreateReviewAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _cacheServiceMock.Received(1)
            .RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
