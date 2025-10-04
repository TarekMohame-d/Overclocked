using System.Net;
using Application.Abstraction.Messaging;
using Application.Features.Brand.Commands.CreateBrand;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Entities;
using Application.Abstraction.Services;
using Application.Abstraction.Repositories;

namespace Unit.Tests.BrandTests.Commands;

public class CreateBrandCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly CreateBrandCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public CreateBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _mediatorMock = Substitute.For<IMediator>();
        _handler = new CreateBrandCommandHandler(
            _unitOfWorkMock,
            _brandRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateBrandCommand
        {
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var brand = new BrandFaker().Generate();

        _brandRepositoryMock.AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _brandRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Brand>(), Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
