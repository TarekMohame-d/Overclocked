using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Commands.UpdateReview;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ReviewAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class UpdateReviewCommandHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateReviewCommandHandler _updateReviewCommandHandler;

    public UpdateReviewCommandHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateReviewCommandHandler = new UpdateReviewCommandHandler(
            _reviewRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateReviewCommandHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new UpdateReviewCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        _reviewRepositoryMock.GetForUpdateAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewCommandHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new UpdateReviewCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Comment"
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetForUpdateAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _updateReviewCommandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        review.DomainEvents.ShouldNotBeEmpty();
    }
}
