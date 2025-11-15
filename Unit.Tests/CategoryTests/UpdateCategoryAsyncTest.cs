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
            ImageUrl = "image.png"
        };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        _eventDispatcherMock.DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _categoryService.UpdateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateCategoryAsync_Should_ReturnSuccess_When_CategoryExist()
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = "image.png"
        };

        Category brand = new CategoryFaker().Generate();

        brand.Name = request.Name;

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _categoryRepositoryMock.Update(Arg.Any<Category>());

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
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1)
            .Update(Arg.Any<Category>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<CategoryUpdatedEvent>(), CancellationToken.None);
    }
}
