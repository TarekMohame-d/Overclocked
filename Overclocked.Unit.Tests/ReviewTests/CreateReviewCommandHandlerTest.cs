using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Commands.CreateReview;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ReviewAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class CreateReviewCommandHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateReviewCommandHandler _createReviewCommandHandler;

    public CreateReviewCommandHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createReviewCommandHandler = new CreateReviewCommandHandler(
            _reviewRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateReviewCommandHandler_Should_ReturnFailure_When_ReviewAlreadyExist()
    {
        // Arrange
        var request = new CreateReviewCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _createReviewCommandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _reviewRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewCommandHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new CreateReviewCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _createReviewCommandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Review>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
