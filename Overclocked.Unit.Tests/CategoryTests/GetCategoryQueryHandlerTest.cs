using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries;
using Overclocked.Application.Category.Queries.GetCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetCategoryQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ICategoryQueries _categoryQueries;

    public GetCategoryQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _categoryQueries = new CategoryQueries(_categoryRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryQueryHandler_Should_ReturnCategory_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var query = new GetCategoryQuery { Id = CategoryId.Create(categoryId) };

        Category category = new CategoryFaker().Generate();
        CategoryResponse categoryDto = category.ToDto();

        _categoryRepositoryMock.GetCategoryByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        Result<CategoryResponse> result = await _categoryQueries.GetCategoryQueryHandler(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Value.ShouldNotBeNull();
        categoryDto.ShouldBeEquivalentTo(result.Value);

        await _categoryRepositoryMock.Received(1)
            .GetCategoryByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryQueryHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var query = new GetCategoryQuery { Id = CategoryId.Create(categoryId) };

        _categoryRepositoryMock.GetCategoryByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result<CategoryResponse> result = await _categoryQueries.GetCategoryQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetCategoryByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>());
    }
}
