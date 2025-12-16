using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.ReviewReply.Commands.DeleteReviewReply;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewReplyTests;

public class DeleteReviewReplyCommandHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteReviewReplyCommandHandler _deleteReviewReplyCommandHandler;

    public DeleteReviewReplyCommandHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteReviewReplyCommandHandler = new DeleteReviewReplyCommandHandler(
            _reviewRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteReviewReplyCommandHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var command = new DeleteReviewReplyCommand
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid()
        };

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _deleteReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyCommandHandler_Should_ReturnFailure_When_ReviewReplyDoesNotExist()
    {
        // Arrange
        var command = new DeleteReviewReplyCommand
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid()
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _deleteReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyCommandHandler_Should_ReturnFailure_When_Unauthorized()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        var reviewReplyId = ReviewReplyId.Create(Guid.NewGuid());

        var command = new DeleteReviewReplyCommand
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReplyId.Value,
            EmployeeId = Guid.NewGuid()
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(ReviewReply.Create(userId, "Reply"));

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _deleteReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Unauthorized);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyCommandHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        var reviewReplyId = ReviewReplyId.Create(Guid.NewGuid());

        var command = new DeleteReviewReplyCommand
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReplyId.Value,
            EmployeeId = userId
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(ReviewReply.Create(userId, "Reply"));

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.SaveChangesAsync(CancellationToken.None)
            .Returns(1);

        // Act
        Result result = await _deleteReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(CancellationToken.None);

        review.ReviewReply.ShouldBeNull();
    }
}
