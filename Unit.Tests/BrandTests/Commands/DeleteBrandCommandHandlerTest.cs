using System.Net;
using Domain.Exceptions;
using Application.Common.Results;
using Application.Features.Brand.Commands.DeleteBrand;
using ArchitectureTests.FakeData;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using BrandEntity = Domain.Entities.Brand;
using Domain.Repositories;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;

namespace Unit.Tests.BrandTests.Commands;

public class DeleteBrandCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly DeleteBrandCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public DeleteBrandCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mediatorMock = Substitute.For<IMediator>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _handler = new DeleteBrandCommandHandler(
            _unitOfWorkMock,
            _brandRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenBrandDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteBrandCommand();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<BrandEntity?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBrandExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = new DeleteBrandCommand();

        var brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<BrandEntity>());

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

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<BrandEntity>());
    }

    [Fact]
    public async Task Handle_WhenDeleteBrandFails_ShouldThrowExceptionAndReturnFailure()
    {
        // Arrange
        var command = new DeleteBrandCommand();

        var brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<BrandEntity>());

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("Delete failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        result.Error.Type.ShouldBe(ErrorType.InternalServerError);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<BrandEntity>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
