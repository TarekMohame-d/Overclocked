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

public class GetAllCategoriesAsyncTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _categoryService;

    public GetAllCategoriesAsyncTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _categoryService = new CategoryService(
            _categoryRepositoryMock,
            Substitute.For<IUnitOfWork>(),
            Substitute.For<IEventDispatcher>()
        );
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_ReturnCategories_When_CategoriesExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();
        List<Category> brands = new CategoryFaker().Generate(3);

        IEnumerable<CategoryListResponse> categoryListResponses = brands.ToDto();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(brands);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _categoryService.GetAllCategoriesAsync(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        brands.Count.ShouldBe(result.Data.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        categoryListResponses.ShouldBeEquivalentTo(result.Data);

        await _categoryRepositoryMock.Received(1).GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllCategoriesAsync_Should_ReturnEmptyList_When_CategoriesDoesNotExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _categoryService.GetAllCategoriesAsync(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1).GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
