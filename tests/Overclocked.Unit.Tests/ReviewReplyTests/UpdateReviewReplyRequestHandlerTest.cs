using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewReplyUseCases.UpdateReviewReply;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewReplyTests;

public class UpdateReviewReplyRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateReviewReplyRequestHandler _updateReviewReplyRequestHandler;

    public UpdateReviewReplyRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateReviewReplyRequestHandler = new UpdateReviewReplyRequestHandler(_reviewRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateReviewReplyRequestHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new UpdateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply",
        };

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _updateReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewReplyRequestHandler_Should_ReturnFailure_When_ReviewReplyDoesNotExist()
    {
        // Arrange
        var request = new UpdateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _updateReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock
            .Received(1)
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), ct: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewReplyRequestHandler_Should_ReturnFailure_When_Unauthorized()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        ReviewReply reviewReply = ReviewReply.Create(userId, "Reply").Value;

        var request = new UpdateReviewReplyRequest
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReply.Id.Value,
            EmployeeId = Guid.NewGuid(),
            Reply = "new Reply",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(reviewReply);

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), ct: Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _updateReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Unauthorized);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewReplyRequestHandler_Should_ReturnSuccess_When_UpdateReviewReplyFailed()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        ReviewReply reviewReply = ReviewReply.Create(userId, "Reply").Value;

        var request = new UpdateReviewReplyRequest
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReply.Id.Value,
            EmployeeId = userId.Value,
            Reply = "    ",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(reviewReply);

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _updateReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateReviewReplyRequestHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        ReviewReply reviewReply = ReviewReply.Create(userId, "Reply").Value;

        var request = new UpdateReviewReplyRequest
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReply.Id.Value,
            EmployeeId = userId.Value,
            Reply = "new Reply",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(reviewReply);

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        _unitOfWorkMock.SaveChangesAsync(CancellationToken.None).Returns(1);

        // Act
        Result result = await _updateReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(CancellationToken.None);

        review.ReviewReply.ShouldNotBeNull();
        review.ReviewReply.Reply.ShouldBe(request.Reply);
    }
}
