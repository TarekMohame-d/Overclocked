using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Product;
using Application.Services.Product.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests;

public class CreateProductAsyncTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly ProductService _productService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateProductAsyncTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _productService = new ProductService(
            _productRepositoryMock,
            _unitOfWorkMock,
            Substitute.For<IEventDispatcher>()
        );
    }

    [Fact]
    public async Task CreateProductAsync_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Discount = 10,
            Price = 100,
            Stock = 10,
            Thumbnail = "Thumbnail",
            Specification = [new CreateProductRequest.Specs { Name = "Name", Value = "Value" }],
            Tags = [Guid.NewGuid()],
        };

        Product product = new ProductFaker().Generate();

        _productRepositoryMock.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Returns(product);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _productService.CreateProductAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _productRepositoryMock.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
