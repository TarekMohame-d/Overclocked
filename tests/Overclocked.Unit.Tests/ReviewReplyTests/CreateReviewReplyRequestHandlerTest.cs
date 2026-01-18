using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewReplyUseCases.CreateReviewReply;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewReplyTests;

public class CreateReviewReplyRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateReviewReplyRequestHandler _createReviewReplyRequestHandler;

    public CreateReviewReplyRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createReviewReplyRequestHandler = new CreateReviewReplyRequestHandler(_reviewRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateReviewReplyRequestHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply",
        };

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _createReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewReplyRequestHandler_Should_ReturnFailure_When_ReplyAlreadyExist()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        review.AddReviewReply(ReviewReply.Create(UserId.Create(), "Reply").Value);

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _createReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewReplyRequestHandler_Should_ReturnFailure_When_CreateReviewReplyFailed()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "       ",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        // Act
        Result result = await _createReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewReplyAsync_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply",
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createReviewReplyRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        review.ReviewReply.ShouldNotBeNull();
        review.ReviewReply.Reply.ShouldBe(request.Reply);
    }
}
