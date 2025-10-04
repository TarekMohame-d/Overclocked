using System.Net;
using Application.Abstraction.Repositories;
using Application.Features.Product.Queries.GetPagedProducts;
using ArchitectureTests.FakeData;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests.Queries;

public class GetPagedProductsQueryHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly GetPagedProductsQueryHandler _handler;

    public GetPagedProductsQueryHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _handler = new GetPagedProductsQueryHandler(_productRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenProductsExist_ShouldReturnProducts()
    {
        // Arrange
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 10
        };
        var products = new ProductFaker().Generate(3);

        var mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<string>())
            .Returns(mockQueryable);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        products.Count.ShouldBe(result.Data.Items.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenProductsDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetPagedProductsQuery();

        var products = new ProductFaker().Generate(0);

        var mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<string>())
            .Returns(mockQueryable);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<string>());
    }
}
