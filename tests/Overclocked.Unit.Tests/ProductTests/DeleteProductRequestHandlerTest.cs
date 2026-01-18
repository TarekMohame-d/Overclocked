using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ProductUseCases.DeleteProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class DeleteProductRequestHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteProductRequestHandler _deleteProductRequestHandler;

    public DeleteProductRequestHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteProductRequestHandler = new DeleteProductRequestHandler(_productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteProductRequestHandler_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var productId = Guid.CreateVersion7();

        var request = new DeleteProductRequest { Id = productId };

        _productRepositoryMock.FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns((Product)null!);

        // Act
        Result result = await _deleteProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1).FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductRequestHandler_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = new DeleteProductRequest { Id = productId };

        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _productRepositoryMock.FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(product);

        _productRepositoryMock.Remove(Arg.Any<Product>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteProductRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productRepositoryMock.Received(1).FindAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1).Remove(Arg.Any<Product>());
    }
}
