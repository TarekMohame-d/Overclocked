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

public class DeleteCategoryAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryRepository _brandRepositoryMock;
    private readonly ICategoryService _brandServices;
    private readonly IFileStorageService _fileStorageServiceMock;

    public DeleteCategoryAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _brandRepositoryMock = Substitute.For<ICategoryRepository>();
        _fileStorageServiceMock = Substitute.For<IFileStorageService>();
        _brandServices = new CategoryService(_brandRepositoryMock, _unitOfWorkMock, _fileStorageServiceMock);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var request = new DeleteCategoryRequest { Id = Guid.NewGuid() };

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category?>(null));

        // Act
        var result = await _brandServices.DeleteCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var request = new DeleteCategoryRequest { Id = Guid.NewGuid() };

        var brand = new CategoryFaker().Generate();

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _brandRepositoryMock.Delete(Arg.Any<Category>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _brandServices.DeleteCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _brandRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _brandRepositoryMock.Received(1)
            .Delete(Arg.Any<Category>());
    }
}
