using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Enums;
using Application.Services;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using ArchitectureTests.FakeData;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class GetPagedProductsAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IProductService _productService;

    public GetPagedProductsAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _productService = new ProductService(_productRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task GetPagedProductsAsync_Should_ReturnProducts_When_ProductsExist()
    {
        // Arrange
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 10
        };
        var products = new ProductFaker().Generate(3);
        var brands = new BrandFaker().Generate(3);

        for (var i = 0; i < products.Count; i++)
        {
            products[i].Brand = brands[i];
            products[i].BrandId = brands[i].Id;
        }

        var mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<Application.Common.Enums.SortDirection>())
            .Returns(mockQueryable);

        // Act
        var result = await _productService.GetPagedProductsAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        products.Count.ShouldBe(result.Data.Items.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<Application.Common.Enums.SortDirection>());
    }

    [Fact]
    public async Task GetPagedProductsAsync_Should_ReturnEmptyList_When_ProductsDoesNotExist()
    {
        // Arrange
        var query = new GetPagedProductsQuery();

        var products = new ProductFaker().Generate(0);

        var mockQueryable = products.BuildMock();

        _productRepositoryMock.GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<Application.Common.Enums.SortDirection>())
            .Returns(mockQueryable);

        // Act
        var result = await _productService.GetPagedProductsAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _productRepositoryMock.Received(1)
            .GetProductsQuery(Arg.Any<ProductSortField>(), Arg.Any<Application.Common.Enums.SortDirection>());
    }
}
