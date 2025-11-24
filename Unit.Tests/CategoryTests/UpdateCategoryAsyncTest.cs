using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Events;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests;

public class UpdateCategoryAsyncTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _categoryService;
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateCategoryAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _categoryService = new CategoryService(_categoryRepositoryMock, _unitOfWorkMock, _eventDispatcherMock);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = "image.png",
        };

        _categoryRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        Result result = await _categoryService.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnSuccess_When_CategoryExistAndNameIsSame()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = "image.png",
        };

        Category category = new CategoryFaker().Generate();

        category.Name = request.Name;

        _categoryRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        _eventDispatcherMock.DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _categoryService.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _categoryRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnFailure_When_CategoryExistAndNameChangedAndNameIsNotUnique()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = "image.png",
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _categoryService.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();

        await _categoryRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnSuccess_When_CategoryExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = "image.png",
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _categoryService.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _categoryRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Category, bool>>>(),
            asNoTracking: Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None);
    }
}
