using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Category.Mapping;
using Application.Features.Category.Queries.GetCategoryById;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using System.Net;
using CategoryEntity = Domain.Entities.Category;

namespace Unit.Tests.CategoryTests.Queries;

public class GetCategoryByIdQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _handler = new GetCategoryByIdQueryHandler(_categoryRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldReturnBrand()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var query = new GetCategoryByIdQuery { Id = categoryId };
        var category = new CategoryFaker().Generate();
        var categoryDto = category.ToDto();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        categoryDto.ShouldBeEquivalentTo(result.Data);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((CategoryEntity)null!);

        var query = new GetCategoryByIdQuery { Id = categoryId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
