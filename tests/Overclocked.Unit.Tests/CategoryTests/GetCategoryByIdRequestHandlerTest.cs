using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.GetCategoryById;
using Overclocked.Application.Features.CategoryUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class GetCategoryByIdRequestHandlerTest
{
    private readonly ICategoryReadRepository _categoryReadRepositoryMock;
    private readonly GetCategoryByIdRequestHandler _getCategoryByIdRequestHandler;

    public GetCategoryByIdRequestHandlerTest()
    {
        _categoryReadRepositoryMock = Substitute.For<ICategoryReadRepository>();

        _getCategoryByIdRequestHandler = new GetCategoryByIdRequestHandler(_categoryReadRepositoryMock);
    }

    [Fact]
    public async Task GetCategoryRequestHandler_Should_ReturnCategory_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = categoryId };

        Category category = new CategoryFaker().Generate();
        CategoryResponse categoryDto = category.ToDto();

        _categoryReadRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        // Act
        Result<CategoryResponse> result = await _getCategoryByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.Value.ShouldNotBeNull();
        categoryDto.ShouldBeEquivalentTo(result.Value);

        await _categoryReadRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryRequestHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var request = new GetCategoryByIdRequest { Id = categoryId };

        _categoryReadRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns((Category)null!);

        // Act
        Result<CategoryResponse> result = await _getCategoryByIdRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryReadRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }
}
