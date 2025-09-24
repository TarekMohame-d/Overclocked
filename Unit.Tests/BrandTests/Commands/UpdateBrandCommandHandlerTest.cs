using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Commands.UpdateBrand;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Repositories;
using Application.Abstraction.Messaging;
using Domain.Entities;
using Application.Abstraction.Services;

namespace Unit.Tests.BrandTests.Commands;

public class UpdateBrandCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly UpdateBrandCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public UpdateBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _mediatorMock = Substitute.For<IMediator>();

        _handler = new UpdateBrandCommandHandler(
            _unitOfWorkMock,
            _brandRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenBrandDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateBrandWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBrandExist_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateBrandWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var brand = new BrandFaker().Generate();

        brand.Name = command.Name;

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Update(Arg.Any<Brand>());

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

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Update(Arg.Any<Brand>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
