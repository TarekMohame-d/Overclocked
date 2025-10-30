using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Services;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests;

public class GetCategoryByIdAsyncTest
{
    private readonly ICategoryRepository _brandRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public GetCategoryByIdAsyncTest()
    {
        _brandRepositoryMock = Substitute.For<ICategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new CategoryService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_ReturnCategory_When_CategoryExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = brandId };
        var brand = new CategoryFaker().Generate();
        var brandDto = brand.ToDto();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        // Act
        var result = await _brandServices.GetCategoryByIdAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Data);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = brandId };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        var result = await _brandServices.GetCategoryByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
