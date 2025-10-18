using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Product.Commands.UpdateProduct;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests.Commands;

public class UpdateProductCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly UpdateProductCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public UpdateProductCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _mediatorMock = Substitute.For<IMediator>();

        _handler = new UpdateProductCommandHandler(
            _productRepositoryMock,
            _unitOfWorkMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateProductWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Test",
            Description = "Test",
            Thumbnail = "Test",
        };

        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _productRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductExist_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateProductWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Test",
            Description = "Test",
            Thumbnail = "Test",
        };

        var product = new ProductFaker().Generate();

        product.Name = command.Name;

        _productRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _productRepositoryMock.Update(Arg.Any<Product>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

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

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
