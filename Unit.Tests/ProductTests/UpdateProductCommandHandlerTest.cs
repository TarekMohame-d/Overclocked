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

public class UpdateProductAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IFileStorageService _fileStorageServiceMock;
    private readonly IProductService _productService;

    public UpdateProductAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _productService = new ProductService(_productRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Discount = 10,
            Price = 100,
            Stock = 10,
            Thumbnail = "Thumbnail",
            Specification = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()]
        };

        // Act
        var result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnSuccess_When_ProductExist()
    {
        // Arrange
        var product = new ProductFaker().Generate();

        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Discount = 10,
            Price = 100,
            Stock = 10,
            Thumbnail = "Thumbnail",
            Specification = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()]
        };

        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Update(Arg.Any<Product>());
    }
}
