using System.Net;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Entities;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using Application.Services;

namespace Unit.Tests.CategoryTests;

public class UpdateCategoryAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryRepository _brandRepositoryMock;
    private readonly ICategoryService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public UpdateCategoryAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<ICategoryRepository>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new CategoryService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        var result = await _brandServices.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnSuccess_When_CategoryExist()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var brand = new CategoryFaker().Generate();

        brand.Name = request.Name;

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Update(Arg.Any<Category>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _brandServices.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Update(Arg.Any<Category>());
    }
}
