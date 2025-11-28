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

public class CreateReviewReplyAsyncTest
{
    private readonly IGenericRepository<ReviewReply> _reviewReplyRepositoryMock;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICacheService _cacheServiceMock;
    private readonly ReviewReplyService _reviewReplyService;

    public CreateReviewReplyAsyncTest()
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
    public async Task CreateReviewReplyAsync_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _reviewReplyService.CreateReviewReplyAsync(request, CancellationToken.None);

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
    public async Task CreateReviewReplyAsync_Should_ReturnFailure_When_ReplyAlreadyExist()
    {
        // Arrange
        var request = new CreateReviewReplyRequest
        {
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Reply = "Reply"
        };

        Review review = new ReviewFaker().Generate();

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        _reviewReplyRepositoryMock.AnyAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _reviewReplyService.CreateReviewReplyAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _reviewReplyRepositoryMock.Received(1)
            .AnyAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
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
            Reply = "Reply"
        };

        Review review = new ReviewFaker().Generate();

        _reviewRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(review);

        _reviewReplyRepositoryMock.AnyAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        var reviewReply = new ReviewReply
        {
            ReviewId = request.ReviewId,
            EmployeeId = request.EmployeeId,
            Reply = request.Reply
        };

        _reviewReplyRepositoryMock.AddAsync(Arg.Any<ReviewReply>(), Arg.Any<CancellationToken>())
            .Returns(reviewReply);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _cacheServiceMock.RemoveAsync(Arg.Any<string>())
            .Returns(Task.FromResult(true));

        // Act
        Result result = await _reviewReplyService.CreateReviewReplyAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Error.ShouldBeNull();

        await _reviewRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Review, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _reviewReplyRepositoryMock.Received(1)
            .AnyAsync(
            Arg.Any<Expression<Func<ReviewReply, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _reviewReplyRepositoryMock.Received(1)
            .AddAsync(Arg.Any<ReviewReply>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _cacheServiceMock.Received(1)
            .RemoveAsync(Arg.Any<string>());
    }
}
