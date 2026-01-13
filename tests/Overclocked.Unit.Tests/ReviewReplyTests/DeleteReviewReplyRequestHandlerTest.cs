using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewReplyUseCases.DeleteReviewReply;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewReplyTests;

public class DeleteReviewReplyRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteReviewReplyRequestHandler _deleteReviewReplyRequestHandler;

    public DeleteReviewReplyRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteReviewReplyRequestHandler = new DeleteReviewReplyRequestHandler(_reviewRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteReviewReplyRequestHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
        };

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _deleteReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyRequestHandler_Should_ReturnFailure_When_ReviewReplyDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _deleteReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyRequestHandler_Should_ReturnFailure_When_Unauthorized()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        ReviewReply reviewReply = ReviewReply.Create(userId, "Reply").Value;

        var request = new DeleteReviewReplyRequest
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReply.Id.Value,
            EmployeeId = Guid.NewGuid(),
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(reviewReply);

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _deleteReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Unauthorized);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyRequestHandler_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var reviewId = ReviewId.Create(Guid.NewGuid());
        ReviewReply reviewReply = ReviewReply.Create(userId, "Reply").Value;

        var request = new DeleteReviewReplyRequest
        {
            ReviewId = reviewId.Value,
            ProductId = Guid.NewGuid(),
            ReplyId = reviewReply.Id.Value,
            EmployeeId = userId.Value,
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        review.AddReviewReply(reviewReply);

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        _unitOfWorkMock.SaveChangesAsync(CancellationToken.None).Returns(1);

        // Act
        Result result = await _deleteReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(CancellationToken.None);

        review.ReviewReply.ShouldBeNull();
    }
}
