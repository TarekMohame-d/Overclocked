using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Product.Queries;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class GetProductQueryHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IReviewRepository _reviewRepositoryMock;
    private readonly IProductQueries _productQueries;

    public GetProductQueryHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _reviewRepositoryMock = Substitute.For<IReviewRepository>();

        _productQueries = new ProductQueries(_productRepositoryMock, _reviewRepositoryMock);
    }

    [Fact]
    public async Task GetProductQueryHandler_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.GetByIdWithDetailsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        var query = new GetProductQuery { Id = Guid.NewGuid() };

        // Act
        Result<ProductResponse> result = await _productQueries.GetProductQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBe(Error.None);

        await _productRepositoryMock.Received(1)
            .GetByIdWithDetailsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _reviewRepositoryMock.DidNotReceive()
            .GetProductRatingsBreakdownAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }
}
