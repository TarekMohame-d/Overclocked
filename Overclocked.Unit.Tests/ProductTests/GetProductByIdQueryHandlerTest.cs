using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Queries.GetProductById;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class GetProductByIdQueryHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly GetProductByIdQueryHandler _getProductByIdQueryHandler;

    public GetProductByIdQueryHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();

        _getProductByIdQueryHandler = new GetProductByIdQueryHandler(_productRepositoryMock, _reviewRepositoryMock);
    }

    [Fact]
    public async Task GetProductByIdQueryHandler_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        var query = new GetProductByIdQuery { Id = Guid.NewGuid() };

        // Act
        Result<ProductResponse> result = await _getProductByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.DidNotReceive()
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }
}
