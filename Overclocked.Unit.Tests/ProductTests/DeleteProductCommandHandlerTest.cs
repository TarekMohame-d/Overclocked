using System.Linq.Expressions;
using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Product.Commands;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class DeleteProductCommandHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IProductCommands _productCommands;

    public DeleteProductCommandHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _productCommands = new ProductCommands(
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

        _productRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Product, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _productCommands.DeleteProductCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Product, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
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

        _productRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Product, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _productCommands.DeleteProductCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _productRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Product, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());
    }
}
