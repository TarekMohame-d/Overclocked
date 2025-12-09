using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Category.Commands;
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
    private readonly ICategoryCommands _categoryCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _categoryCommands = new CategoryCommands(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var command = new DeleteCategoryCommand(CategoryId.Create(categoryId));

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result result = await _categoryCommands.DeleteCategoryCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategoryCommandHandler_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var categoryId = Guid.CreateVersion7();
        var command = new DeleteCategoryCommand(CategoryId.Create(categoryId));

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.Delete(Arg.Any<Category>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _categoryCommands.DeleteCategoryCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1)
            .Delete(Arg.Any<Category>());
    }
}
