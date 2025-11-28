using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.DomainServices;
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

public class UpdateProductAsyncTest
{
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IReviewService _reviewServiceMock;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateProductAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _reviewServiceMock = Substitute.For<IReviewService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();

        _productService = new ProductService(
            _productRepositoryMock,
            _reviewServiceMock,
            _unitOfWorkMock,
            _eventDispatcherMock);
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
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
            Specification = [new UpdateProductRequestBody.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()],
            Images = null,
        };

        _productRepositoryMock.GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnSuccess_When_ProductExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        Product product = new ProductFaker().Generate();

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
            Specification = [new UpdateProductRequestBody.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()],
        };

        _productRepositoryMock.GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _productRepositoryMock.Received(1)
            .GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<ProductUpdatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnSuccess_When_ProductExistAndNameChangedAndNameIsNotUnique()
    {
        // Arrange
        Product product = new ProductFaker().Generate();

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
            Specification = [new UpdateProductRequestBody.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()],
        };

        _productRepositoryMock.GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _productRepositoryMock.Received(1)
            .GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive()
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<ProductUpdatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductAsync_Should_ReturnSuccess_When_ProductExistAndImagesChanged()
    {
        // Arrange
        Product product = new ProductFaker().Generate();

        product.ProductImages =
        [
            new ProductImage
            {
                Image = "image1.png",
                ProductId = product.Id,
            },
            new ProductImage
            {
                Image = "image2.png",
                ProductId = product.Id,
            },
            new ProductImage
            {
                Image = "image3.png",
                ProductId = product.Id,
            },
        ];

        var request = new UpdateProductRequest
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = product.Name,
            Description = "Product Description",
            Discount = 10,
            Price = 100,
            Stock = 10,
            Thumbnail = "Thumbnail",
            Specification = [new UpdateProductRequestBody.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()],
            Images = ["image1.png", "image2.png", "image4.png"],
        };

        _productRepositoryMock.GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _eventDispatcherMock.DispatchAsync(Arg.Any<ProductUpdatedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _productService.UpdateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _productRepositoryMock.Received(1)
            .GetProductForUpdateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _productRepositoryMock.DidNotReceive()
            .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<ProductUpdatedEvent>(), Arg.Any<CancellationToken>());
    }
}
