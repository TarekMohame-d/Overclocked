using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Product.Commands.DeleteProduct;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests.Commands;

public class DeleteProductCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly DeleteProductCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public DeleteProductCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mediatorMock = Substitute.For<IMediator>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _handler = new DeleteProductCommandHandler(
            _unitOfWorkMock,
            _productRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProductDoesNotExists()
    {
        // Arrange
        var command = new DeleteProductCommand
        {
            Id = Guid.CreateVersion7()
        };

        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ProductExists()
    {
        // Arrange
        var command = new DeleteProductCommand
        {
            Id = Guid.CreateVersion7()
        };

        var product = new ProductFaker().Generate();

        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.Delete(Arg.Any<Product>());

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        _productRepositoryMock.Received(1)
            .Delete(Arg.Any<Product>());
    }
}
