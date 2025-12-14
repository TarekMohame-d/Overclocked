using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries.GetCategoryById;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetCategoryByIdQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly GetCategoryByIdQueryHandler _getCategoryByIdQueryHandler;

    public GetCategoryByIdQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _getCategoryByIdQueryHandler = new GetCategoryByIdQueryHandler(_categoryRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryQueryHandler_Should_ReturnCategory_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var query = new GetCategoryByIdQuery { Id = CategoryId.Create(categoryId) };

        Category category = new CategoryFaker().Generate();
        CategoryResponse categoryDto = category.ToDto();

        _categoryRepositoryMock.GetByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        Result<CategoryResponse> result = await _getCategoryByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldNotBeNull();
        categoryDto.ShouldBeEquivalentTo(result.Value);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryQueryHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var query = new GetCategoryByIdQuery { Id = CategoryId.Create(categoryId) };

        _categoryRepositoryMock.GetByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result<CategoryResponse> result = await _getCategoryByIdQueryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(
            Arg.Any<CategoryId>(),
            Arg.Any<CancellationToken>());
    }
}
