using System.Linq.Expressions;
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
            ImageUrl = "image.png",
        };

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        Result result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnSuccess_When_BrandExistAndNameIsSame()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Brand Name",
            ImageUrl = "image.png",
        };

        Brand brand = new BrandFaker().Generate();

        brand.Name = request.Name;

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

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
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnFailure_When_BrandExistAndNameChangedAndNameIsNotUnique()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Brand Name",
            ImageUrl = "image.png",
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnSuccess_When_BrandExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Brand Name",
            ImageUrl = "image.png",
        };

        Brand brand = new BrandFaker().Generate();

        _brandRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _eventDispatcherMock.DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _brandRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Brand, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Brand, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<BrandUpdatedEvent>(), CancellationToken.None);
    }
}
