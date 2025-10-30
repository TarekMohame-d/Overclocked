using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Features.Brand.Mapping;
using Application.Services;
using Application.Services.Brand;
using Application.Services.Brand.DTOs.Request;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.BrandTests;

public class GetAllBrandsAsyncTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IBrandService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public GetAllBrandsAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new BrandService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task GetAllBrandsAsync_Should_ReturnBrands_When_BrandsExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();
        var brands = new BrandFaker().Generate(3);

        var brandDtos = brands.ToDto();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(brands);

        // Act
        var result = await _brandServices.GetAllBrandsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        brands.Count.ShouldBe(result.Data.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        brandDtos.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllBrandsAsync_Should_ReturnEmptyList_When_BrandsDoesNotExist()
    {
        // Arrange
        var request = new GetAllBrandsRequest();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _brandServices.GetAllBrandsAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
