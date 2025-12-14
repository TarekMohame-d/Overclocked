using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetAllCategoriesQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly GetAllCategoriesQueryHandler _getAllCategoriesQueryHandler;

    public GetAllCategoriesQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _getAllCategoriesQueryHandler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryListQueryHandler_Should_ReturnCategorys_When_CategoriesExist()
    {
        // Arrange
        var query = new GetAllCategoriesQuery();
        List<Category> categories = new CategoryFaker().Generate(3);

        IEnumerable<CategoryListResponse> categoryListResponses = categories.ToDto();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _getAllCategoriesQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        categories.Count.ShouldBe(result.Value.Count());
        categoryListResponses.ShouldBeEquivalentTo(result.Value);

        await _categoryRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryListQueryHandler_Should_ReturnEmptyList_When_CategoriesDoesNotExist()
    {
        // Arrange
        var query = new GetAllCategoriesQuery();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _getAllCategoriesQueryHandler
            .Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);

        await _categoryRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }
}
