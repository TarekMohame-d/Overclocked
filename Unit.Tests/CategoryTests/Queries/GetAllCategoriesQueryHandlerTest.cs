using Application.Features.Category.Mapping;
using Application.Features.Category.Queries.GetAllCategories;
using ArchitectureTests.FakeData;
using Domain.Repositories;
using NSubstitute;
using Shouldly;
using System.Net;
namespace Unit.Tests.CategoryTests.Queries;

public class GetAllCategoriesQueryHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _handler = new GetAllCategoriesQueryHandler(_categoryRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ShouldReturnCategories()
    {
        // Arrange
        var query = new GetAllCategoriesQuery();
        var categories = new CategoryFaker().Generate(3);

        var categoriesDtos = categories.ToDto();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(categories);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        categories.Count.ShouldBe(result.Data.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        categoriesDtos.ShouldBeEquivalentTo(result.Data);

        await _categoryRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenBrandsDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllCategoriesQuery();

        _categoryRepositoryMock.GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.ShouldBeEmpty();
        result.Data.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1)
            .GetAllAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }
}
