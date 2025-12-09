using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetCategoryListQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ICategoryQueries _categoryQueries;

    public GetCategoryListQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _categoryQueries = new CategoryQueries(_categoryRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryListQueryHandler_Should_ReturnCategorys_When_CategoriesExist()
    {
        // Arrange
        var query = new GetCategoryListQuery();
        List<Category> categories = new CategoryFaker().Generate(3);

        IEnumerable<CategoryListResponse> categoryListResponses = categories.ToDto();

        _categoryRepositoryMock.GetCategoryListAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _categoryQueries
            .GetCategoryListQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        categories.Count.ShouldBe(result.Value.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        categoryListResponses.ShouldBeEquivalentTo(result.Value);

        await _categoryRepositoryMock.Received(1)
            .GetCategoryListAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryListQueryHandler_Should_ReturnEmptyList_When_CategoriesDoesNotExist()
    {
        // Arrange
        var query = new GetCategoryListQuery();

        _categoryRepositoryMock.GetCategoryListAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _categoryQueries
            .GetCategoryListQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1)
            .GetCategoryListAsync(Arg.Any<CancellationToken>());
    }
}
