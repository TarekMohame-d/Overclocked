using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class GetPagedProductsQueryHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly GetPagedProductsQueryHandler _getPagedProductsQueryHandler;

    public GetPagedProductsQueryHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _getPagedProductsQueryHandler = new GetPagedProductsQueryHandler(_productRepositoryMock);
    }

    [Fact]
    public async Task GetPagedProductsQueryHandler_Should_ReturnEmptyPagedResult_When_ProductsDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.CountAsync(
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>())
            .Returns(0);

        var request = new GetPagedProductsRequest();
        var query = GetPagedProductsQuery.ToQuery(request);

        // Act
        Result<PagedResult<ProductPagedResponse>> result = await _getPagedProductsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _productRepositoryMock.Received(1)
            .CountAsync(
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedProductsQueryHandler_Should_ReturnResults_When_ProductsExists()
    {
        // Arrange
        var request = new GetPagedProductsRequest();
        var query = GetPagedProductsQuery.ToQuery(request);

        List<Product> product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(10);

        _productRepositoryMock.CountAsync(
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>())
            .Returns(10);


        _productRepositoryMock.GetPagedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<ProductSortField>(),
            Arg.Any<Application.Common.Enums.SortDirection>(),
            Arg.Any<CancellationToken>())
            .Returns(product);

        // Act
        Result<PagedResult<ProductPagedResponse>> result = await _getPagedProductsQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Items.ShouldNotBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _productRepositoryMock.Received(1)
            .CountAsync(
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetPagedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<Guid>(),
            Arg.Any<ProductSortField>(),
            Arg.Any<Application.Common.Enums.SortDirection>(),
            Arg.Any<CancellationToken>());
    }
}
