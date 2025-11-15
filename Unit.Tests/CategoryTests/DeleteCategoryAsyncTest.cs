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

public class DeleteCategoryAsyncTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _categoryService;
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteCategoryAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _categoryService = new CategoryService(_categoryRepositoryMock, _unitOfWorkMock, _eventDispatcherMock);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var request = new DeleteCategoryRequest
        {
            Id = Guid.CreateVersion7()
        };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        _eventDispatcherMock.DispatchAsync(Arg.Any<CategoryDeletedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _categoryService.DeleteCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<CategoryDeletedEvent>(), CancellationToken.None);
    }

    [Fact]
    public async Task DeleteCategoryAsync_Should_ReturnSuccess_When_CategoryExists()
    {
        // Arrange
        var request = new DeleteCategoryRequest
        {
            Id = Guid.CreateVersion7()
        };

        Category brand = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(brand);

        _categoryRepositoryMock.Delete(Arg.Any<Category>());

        _eventDispatcherMock.DispatchAsync(Arg.Any<CategoryDeletedEvent>(), CancellationToken.None)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _categoryService.DeleteCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1)
            .Delete(Arg.Any<Category>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<CategoryDeletedEvent>(), CancellationToken.None);
    }
}
