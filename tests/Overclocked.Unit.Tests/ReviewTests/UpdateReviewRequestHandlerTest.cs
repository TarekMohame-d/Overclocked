using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.UpdateReview;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class UpdateReviewRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateReviewRequestHandler _updateReviewRequestHandler;

    public UpdateReviewRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateReviewRequestHandler = new UpdateReviewRequestHandler(
            _reviewRepositoryMock,
            _productReadRepositoryMock,
            _unitOfWorkMock
        );
    }

    [Fact]
    public async Task UpdateReviewRequestHandler_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        var request = new UpdateReviewRequest
        {
            ReviewId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewRequestHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new UpdateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _updateReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewRequestHandler_Should_ReturnFailure_When_UpdateReviewFailed()
    {
        // Arrange
        var request = new UpdateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 5,
            Comment = "    ",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _updateReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewRequestHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new UpdateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _updateReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        review.DomainEvents.ShouldNotBeEmpty();
    }
}
