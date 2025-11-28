using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.ReviewReply;
using Application.Services.ReviewReply.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ReviewReplyTests;

public class DeleteReviewReplyAsyncTest
{
    private readonly IGenericRepository<ReviewReply> _reviewReplyRepositoryMock;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewReplyService _reviewReplyService;

    public DeleteReviewReplyAsyncTest()
    {
        _reviewReplyRepositoryMock = Substitute.For<IGenericRepository<ReviewReply>>();
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cacheServiceMock = Substitute.For<ICacheService>();

        _reviewReplyService = new ReviewReplyService(
            _reviewReplyRepositoryMock,
            _reviewRepositoryMock,
            _unitOfWorkMock,
            _cacheServiceMock);
    }

    [Fact]
    public async Task DeleteReviewReplyAsync_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid()
        };

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _reviewReplyService.DeleteReviewReplyAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyAsync_Should_ReturnFailure_When_ReviewReplyDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid()
        };

        Review review = new ReviewFaker().Generate();

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        _reviewReplyRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((ReviewReply)null!);

        // Act
        Result result = await _reviewReplyService.DeleteReviewReplyAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _reviewReplyRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewReplyAsync_Should_ReturnSuccess_When_AllValid()
    {
        // Arrange
        var request = new DeleteReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid()
        };

        Review review = new ReviewFaker().Generate();

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        var reviewReply = new ReviewReply
        {
            ReviewId = request.ReviewId,
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        _reviewReplyRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(reviewReply);

        _unitOfWorkMock.CompleteAsync(CancellationToken.None)
            .Returns(1);

        _cacheServiceMock.RemoveAsync(Arg.Any<string>())
            .Returns(Task.FromResult(true));

        // Act
        Result result = await _reviewReplyService.DeleteReviewReplyAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _reviewReplyRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        _reviewReplyRepositoryMock.Received(1)
            .Delete(Arg.Any<ReviewReply>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(CancellationToken.None);

        await _cacheServiceMock.Received(1)
            .RemoveAsync(Arg.Any<string>());
    }
}
