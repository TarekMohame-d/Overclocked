using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Features.Product.Commands.CreateProduct;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.ProductTests.Commands;

public class CreateProductCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly CreateProductCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public CreateProductCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _mediatorMock = Substitute.For<IMediator>();
        _handler = new CreateProductCommandHandler(
            _productRepositoryMock,
            _unitOfWorkMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_Should_Success_When_ThereIsNoError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test",
            Description = "Test",
            Thumbnail = "Test",
        };

        var product = new ProductFaker().Generate();

        _productRepositoryMock.AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _productRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
