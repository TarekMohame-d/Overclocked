using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.GetAllCategories;
using Overclocked.Application.Features.CategoryUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetAllCategoriesRequestHandlerTest
{
    private readonly ICategoryReadRepository _brandReadRepositoryMock;
    private readonly GetAllCategoriesRequestHandler _getAllCategoriesRequestHandler;

    public GetAllCategoriesRequestHandlerTest()
    {
        _brandReadRepositoryMock = Substitute.For<ICategoryReadRepository>();

        _getAllCategoriesRequestHandler = new GetAllCategoriesRequestHandler(_brandReadRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryListRequestHandler_Should_ReturnCategories_When_CategoriesExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();
        List<Category> categories = new CategoryFaker().Generate(3);

        IEnumerable<CategoryListResponse> brandListResponses = categories.ToDto();

        _brandReadRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(categories);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _getAllCategoriesRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        categories.Count.ShouldBe(result.Value.Count());

        await _brandReadRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryListRequestHandler_Should_ReturnEmptyList_When_CategoriesDoesNotExist()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();

        _brandReadRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<IEnumerable<CategoryListResponse>> result = await _getAllCategoriesRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldBeEmpty();
        result.Value.Count().ShouldBe(0);

        await _brandReadRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }
}
