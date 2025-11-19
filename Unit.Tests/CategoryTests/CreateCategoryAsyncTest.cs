using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Category;
using Application.Services.Category.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests;

public class CreateCategoryAsyncTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateCategoryAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _categoryService = new CategoryService(
            _categoryRepositoryMock,
            _unitOfWorkMock,
            Substitute.For<IEventDispatcher>()
        );
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Category Name", ImageUrl = "image.png" };

        Category brand = new CategoryFaker().Generate();

        _categoryRepositoryMock.AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>()).Returns(brand);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _categoryService.CreateCategoryAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _categoryRepositoryMock.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
