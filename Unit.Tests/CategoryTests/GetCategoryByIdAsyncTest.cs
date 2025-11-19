using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Application.Services.Category.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests;

public class GetCategoryByIdAsyncTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _categoryService;

    public GetCategoryByIdAsyncTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _categoryService = new CategoryService(
            _categoryRepositoryMock,
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IEventDispatcher>()
        );
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_ReturnCategory_When_CategoryExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = brandId };
        Category brand = new CategoryFaker().Generate();
        CategoryResponse brandDto = brand.ToDto();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>()).Returns(brand);

        // Act
        Result<CategoryResponse> result = await _categoryService.GetCategoryByIdAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        brandDto.ShouldBeEquivalentTo(result.Data);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryByIdAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var brandId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = brandId };

        _categoryRepositoryMock
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result<CategoryResponse> result = await _categoryService.GetCategoryByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
