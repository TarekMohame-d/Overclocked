using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class DeleteProductCommandHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteProductCommandHandler _deleteProductCommandHandler;

    public DeleteProductCommandHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteProductCommandHandler = new DeleteProductCommandHandler(
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteProductCommandHandler_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var productId = Guid.CreateVersion7();

        var command = new DeleteProductCommand
        {
            Id = productId
        };

        _productRepositoryMock.FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _deleteProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductCommandHandler_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var command = new DeleteProductCommand
        {
            Id = productId
        };

        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _productRepositoryMock.FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productRepositoryMock.Received(1)
            .FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());
    }
}
