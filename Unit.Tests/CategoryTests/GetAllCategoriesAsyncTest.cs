using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Features.Category.Mapping;
using Application.Services;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests;

public class GetAllCategorysAsyncTest
{
    private readonly ICategoryRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public GetAllCategorysAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<ICategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new CategoryService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_ReturnCategorys_When_CategorysExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();
        var brands = new CategoryFaker().Generate(3);

        var brandDtos = brands.ToDto();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(brands);

        // Act
        var result = await _brandServices.GetAllCategoriesAsync(request, CancellationToken.None);

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
    public async Task GetAllCategoriesAsync_Should_ReturnEmptyList_When_CategorysDoesNotExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();

        _brandRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _brandServices.GetAllCategoriesAsync(request, CancellationToken.None);

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
