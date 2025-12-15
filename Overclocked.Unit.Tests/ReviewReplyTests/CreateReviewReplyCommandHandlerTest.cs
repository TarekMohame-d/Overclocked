using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.ReviewReply.Commands.CreateReviewReply;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewReplyTests;

public class CreateReviewReplyCommandHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateReviewReplyCommandHandler _createReviewReplyCommandHandler;

    public CreateReviewReplyCommandHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createReviewReplyCommandHandler = new CreateReviewReplyCommandHandler(
            _reviewRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateReviewReplyCommandHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var command = new CreateReviewReplyCommand
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _createReviewReplyCommandHandler.Handle(command, CancellationToken.None);

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
    public async Task CreateReviewReplyCommandHandler_Should_ReturnFailure_When_ReplyAlreadyExist()
    {
        // Arrange
        var command = new CreateReviewReplyCommand
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        review.AddReviewReply(ReviewReply.Create(ReviewReplyId.Create(), UserId.Create(), "Reply"));

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        // Act
        Result result = await _createReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateReviewReplyAsync_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var command = new CreateReviewReplyCommand
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _createReviewReplyCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1)
            .GetForUpdateAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        review.ReviewReply.ShouldNotBeNull();
        review.ReviewReply.Reply.ShouldBe(command.Reply);
    }
}
