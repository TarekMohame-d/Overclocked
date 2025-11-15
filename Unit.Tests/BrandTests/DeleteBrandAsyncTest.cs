using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Events;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests;

public class DeleteBrandAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly BrandService _brandServices;
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteBrandAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _eventDispatcherMock);
    }

    [Fact]
    public async Task DeleteBrandAsync_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var request = new DeleteBrandRequest
        {
            Id = Guid.CreateVersion7()
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        _eventDispatcherMock.DispatchAsync(Arg.Any<BrandDeletedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _brandServices.DeleteBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<BrandDeletedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task DeleteBrandAsync_Should_ReturnSuccess_When_BrandExists()
    {
        // Arrange
        var request = new DeleteBrandRequest
        {
            Id = Guid.CreateVersion7()
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<Brand>());

        _eventDispatcherMock.DispatchAsync(Arg.Any<BrandDeletedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandServices.DeleteBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<Brand>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<BrandDeletedEvent>(), CancellationToken.None);
    }
}
