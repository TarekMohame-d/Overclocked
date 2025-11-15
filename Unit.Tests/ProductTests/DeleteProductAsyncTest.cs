using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Events;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class DeleteProductAsyncTest
{
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteProductAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _productService = new ProductService(_productRepositoryMock, _unitOfWorkMock, _eventDispatcherMock);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            Id = Guid.CreateVersion7()
        };

        _productRepositoryMock.GetProductWithImagesAsync(request.Id, CancellationToken.None)
            .Returns((Product)null!);

        // Act
        Result result = await _productService.DeleteProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetProductWithImagesAsync(request.Id, CancellationToken.None);
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnSuccess_When_ProductExistsAndNoProductImages()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            Id = Guid.CreateVersion7()
        };

        Product product = new ProductFaker().Generate();

        _productRepositoryMock.GetProductWithImagesAsync(request.Id, CancellationToken.None)
            .Returns(product);

        _eventDispatcherMock.DispatchAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _productService.DeleteProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _productRepositoryMock.Received(1)
            .GetProductWithImagesAsync(request.Id, CancellationToken.None);

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductAsync_Should_ReturnSuccess_When_ProductExistsAndHasProductImages()
    {
        // Arrange
        var request = new DeleteProductRequest
        {
            Id = Guid.CreateVersion7()
        };

        Product product = new ProductFaker().Generate();
        product.ProductImages =
        [
            new ProductImage
            {
                Id = Guid.CreateVersion7(),
                Image = "image.png",
                ProductId = product.Id
            }
        ];

        _productRepositoryMock.GetProductWithImagesAsync(request.Id, CancellationToken.None)
            .Returns(product);

        _eventDispatcherMock.DispatchAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _productService.DeleteProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _productRepositoryMock.Received(1)
            .GetProductWithImagesAsync(request.Id, CancellationToken.None);

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<ProductDeletedEvent>(), Arg.Any<CancellationToken>());
    }
}
