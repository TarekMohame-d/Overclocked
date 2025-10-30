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

public class DeleteBrandAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IBrandService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public DeleteBrandAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task DeleteBrandAsync_Should_ReturnFailure_When_BrandDoesNotExists()
    {
        // Arrange
        var request = new DeleteBrandRequest { Id = Guid.NewGuid() };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Brand?>(null));

        // Act
        var result = await _brandServices.DeleteBrandAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteBrandAsync_Should_ReturnSuccess_When_BrandExists()
    {
        // Arrange
        var request = new DeleteBrandRequest { Id = Guid.NewGuid() };

        var brand = new BrandFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<Brand>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _brandServices.DeleteBrandAsync(request, CancellationToken.None);

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
    }
}
