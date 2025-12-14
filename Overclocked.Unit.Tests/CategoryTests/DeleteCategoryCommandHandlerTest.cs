using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Commands.DeleteCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class DeleteCategoryCommandHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteCategoryCommandHandler _deleteCategoryCommandHandler;

    public DeleteCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _deleteCategoryCommandHandler = new DeleteCategoryCommandHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var command = new DeleteCategoryCommand
        {
            Id = categoryId
        };

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result result = await _deleteCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var command = new DeleteCategoryCommand
        {
            Id = categoryId
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.Delete(Arg.Any<Category>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _deleteCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1)
            .Delete(Arg.Any<Category>());
    }
}
