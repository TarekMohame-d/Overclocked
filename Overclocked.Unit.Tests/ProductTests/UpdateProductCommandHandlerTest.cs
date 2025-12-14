using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class UpdateProductCommandHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;

    public UpdateProductCommandHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateProductCommandHandler = new UpdateProductCommandHandler(
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateProductCommandHandler_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Price = 7200,
            Discount = 0.0m,
            StockQuantity = 32,
            Thumbnail = "Thumbnail",
            Specifications = [("Name", "Value")],
            Tags = [Guid.NewGuid()],
            Images = null
        };

        _productRepositoryMock.GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductCommandHandler_Should_ReturnSuccess_When_ProductExistAndNewNameIsUnique()
    {
        // Arrange
        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Price = 7200,
            Discount = 0.0m,
            StockQuantity = 32,
            Thumbnail = "Thumbnail",
            Specifications = [("Name", "Value")],
            Tags = [Guid.NewGuid()],
            Images = null
        };

        _productRepositoryMock.GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productRepositoryMock.Received(1)
            .GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductCommandHandler_Should_ReturnSuccess_When_ProductExistAndNameIsNotUnique()
    {
        // Arrange
        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Price = 7200,
            Discount = 0.0m,
            StockQuantity = 32,
            Thumbnail = "Thumbnail",
            Specifications = [("Name", "Value")],
            Tags = [Guid.NewGuid()],
            Images = null
        };

        _productRepositoryMock.GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _productRepositoryMock.Received(1)
            .GetForUpdateAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<CancellationToken>());
    }
}
