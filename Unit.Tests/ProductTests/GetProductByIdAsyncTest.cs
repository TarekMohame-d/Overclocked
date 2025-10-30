using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class GetProductByIdAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IProductService _productService;

    public GetProductByIdAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _productService = new ProductService(_productRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_ReturnProduct_When_ProductExists()
    {
        // Arrange
        _productRepositoryMock.GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Substitute.For<ProductResponse>());

        var request = new GetProductByIdRequest { Id = Guid.NewGuid() };

        // Act
        var result = await _productService.GetProductByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();

        await _productRepositoryMock.Received(1)
            .GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductByIdAsync_Should_ReturnFailure_When_ProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.GetProductAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ProductResponse)null!);

        var request = new GetProductByIdRequest { Id = Guid.NewGuid() };

        // Act
        var result = await _productService.GetProductByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
    }
}
