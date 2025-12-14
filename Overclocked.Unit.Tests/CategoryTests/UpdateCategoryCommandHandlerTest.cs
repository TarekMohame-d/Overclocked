using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class UpdateCategoryCommandHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateCategoryCommandHandler _updateCategoryCommandHandler;

    public UpdateCategoryCommandHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateCategoryCommandHandler = new UpdateCategoryCommandHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "image.png"
        };

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result result = await _updateCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_Should_ReturnSuccess_When_CategoryExistAndNameIsSame()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "image.png"
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _updateCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_Should_ReturnFailure_When_CategoryExistAndNewNameAlreadyExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "image.png"
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _updateCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryCommandHandler_Should_ReturnSuccess_When_CategoryExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "image.png"
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _updateCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1)
            .FindAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
