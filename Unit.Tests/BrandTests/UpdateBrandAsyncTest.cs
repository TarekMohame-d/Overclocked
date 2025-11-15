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

public class UpdateBrandAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly BrandService _brandServices;
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateBrandAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _eventDispatcherMock);
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        _eventDispatcherMock.DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnSuccess_When_BrandExist()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Brand Name",
            ImageUrl = "image.png"
        };

        Brand brand = new BrandFaker().Generate();

        brand.Name = request.Name;

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Update(Arg.Any<Brand>());

        _eventDispatcherMock.DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

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

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None);
    }
}
