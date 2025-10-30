using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class DeleteProductAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IProductService _productService;

    public DeleteProductAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _productService = new ProductService(_productRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            Id = Guid.CreateVersion7()
        };

        _productRepositoryMock.GetByIdWithImagesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        // Act
        var result = await _productService.DeleteProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetByIdWithImagesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            Id = Guid.CreateVersion7()
        };

        var product = new ProductFaker().Generate();

        _productRepositoryMock.GetByIdWithImagesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _productService.DeleteProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _productRepositoryMock.Received(1)
            .GetByIdWithImagesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());
    }
}
