using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.ProductTests;

public class CreateProductCommandHandlerTest
{
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateProductCommandHandler _createProductCommandHandler;

    public CreateProductCommandHandlerTest()
    {
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _createProductCommandHandler = new CreateProductCommandHandler(
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateProductCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Thumbnail = "Thumbnail",
            Description = "Description",
            Price = 7200,
            StockQuantity = 45,
            Discount = 0.0m,
            Tags = [Guid.NewGuid()],
            Images = null,
            Specifications = [("Name", "Value")]
        };

        Product product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        _productRepositoryMock.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _createProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _productRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
