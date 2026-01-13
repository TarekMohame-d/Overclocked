using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.DeleteCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class DeleteCategoryRequestHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteCategoryRequestHandler _deleteCategoryRequestHandler;

    public DeleteCategoryRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _deleteCategoryRequestHandler = new DeleteCategoryRequestHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteCategoryRequestHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var request = new DeleteCategoryRequest { Id = categoryId };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns((Category)null!);

        // Act
        Result result = await _deleteCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryRequestHandler_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var request = new DeleteCategoryRequest { Id = categoryId };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        _categoryRepositoryMock.Remove(Arg.Any<Category>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        category.DomainEvents.ShouldNotBeEmpty();

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1).Remove(Arg.Any<Category>());
    }
}
