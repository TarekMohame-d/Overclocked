using System.Net;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Entities;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using Application.Services;

namespace Unit.Tests.BrandTests;

public class UpdateBrandAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public UpdateBrandAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Brand)null!);

        // Act
        var result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateBrandAsync_Should_ReturnSuccess_When_BrandExist()
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var brand = new BrandFaker().Generate();

        brand.Name = request.Name;

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Update(Arg.Any<Brand>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _brandServices.UpdateBrandAsync(request, CancellationToken.None);

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
    }
}
