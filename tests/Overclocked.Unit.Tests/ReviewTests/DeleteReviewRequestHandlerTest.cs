using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.DeleteReview;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ReviewTests;

public class DeleteReviewRequestHandlerTest
{
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteReviewRequestHandler _deleteReviewRequestHandler;

    public DeleteReviewRequestHandlerTest()
    {
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteReviewRequestHandler = new DeleteReviewRequestHandler(_reviewRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteReviewRequestHandler_Should_ReturnFailure_When_ReviewDoesNotExist()
    {
        // Arrange
        var request = new DeleteReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
        };

        _reviewRepositoryMock
            .GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((Review)null!);

        // Act
        Result result = await _deleteReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteReviewRequestHandler_Should_ReturnSuccess_When_ReviewExists()
    {
        // Arrange
        var request = new DeleteReviewRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
        };

        Review review = new ReviewFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _reviewRepositoryMock.GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>()).Returns(review);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteReviewRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _reviewRepositoryMock.Received(1).GetAsync(Arg.Any<Expression<Func<Review, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        review.DomainEvents.ShouldNotBeEmpty();
    }
}
