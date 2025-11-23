using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Application.Common.Results;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using MockQueryable;
using NSubstitute;
using Shouldly;
using SortDirection = Application.Common.Enums.SortDirection;

namespace Unit.Tests.ProductTests;

public class GetPagedProductsAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly ProductService _productService;

    public GetPagedProductsAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _productService = new ProductService(
            _productRepositoryMock,
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IEventDispatcher>());
    }

    [Fact]
    public async Task GetPagedProductsAsync_Should_ReturnProducts_When_ProductsExist()
    {
        // Arrange
        var request = new GetPagedProductsRequest { Page = 1, PageSize = 10 };
        List<Product> products = new ProductFaker().Generate(3);
        List<Brand> brands = new BrandFaker().Generate(3);

        for(var i = 0; i < products.Count; i++)
        {
            products[i].Brand = brands[i];
            products[i].BrandId = brands[i].Id;
        }

        IQueryable<Product> mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<SortDirection>())
            .Returns(mockQueryable);

        // Act
        Result<PagedResult<ProductListResponse>> result = await _productService
            .GetPagedProductsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        products.Count.ShouldBe(result.Data.Items.Count);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<SortDirection>());
    }

    [Fact]
    public async Task GetPagedProductsAsync_Should_ReturnEmptyList_When_ProductsDoesNotExist()
    {
        // Arrange
        var request = new GetPagedProductsRequest();

        List<Product> products = new ProductFaker().Generate(0);

        IQueryable<Product> mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<SortDirection>())
            .Returns(mockQueryable);

        // Act
        Result<PagedResult<ProductListResponse>> result = await _productService
            .GetPagedProductsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<SortDirection>());
    }
}
