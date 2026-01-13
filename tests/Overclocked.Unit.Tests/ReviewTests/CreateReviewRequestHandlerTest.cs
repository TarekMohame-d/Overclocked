using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.CreateReview;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class CreateReviewRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateReviewRequestHandler _createReviewRequestHandler;

    public CreateReviewRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createReviewRequestHandler = new CreateReviewRequestHandler(
            _reviewRepositoryMock,
            _productReadRepositoryMock,
            _unitOfWorkMock
        );
    }

    [Fact]
    public async Task CreateReviewRequestHandler_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewRequestHandler_Should_ReturnFailure_When_ReviewAlreadyExist()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        _reviewRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _createReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock
            .Received(1)
            .ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewRequestHandler_Should_ReturnFailure_When_CreateReviewFailed()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 8,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        _reviewRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock
            .Received(1)
            .ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewRequestHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new CreateReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 4,
            Comment = "Comment",
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        _reviewRepositoryMock.ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(false);

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock
            .Received(1)
            .ExistsAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        _reviewRepositoryMock.Received(1).Add(Arg.Any<Review>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
