using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.UpdateCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class UpdateCategoryRequestHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateCategoryRequestHandler _updateCategoryRequestHandler;

    public UpdateCategoryRequestHandlerTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _updateCategoryRequestHandler = new UpdateCategoryRequestHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnFailure_When_ImageUrlIsInvalid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "  ",
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnFailure_When_CategoryDoesNotExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns((Category)null!);

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnSuccess_When_CategoryExistWithSameName()
    {
        // Arrange
        Category category = new CategoryFaker().Generate();

        var request = new UpdateCategoryRequest
        {
            Id = category.Id.Value,
            Name = category.Name,
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnFailure_When_CategoryExistAndNewNameAlreadyExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategoryRequestHandler_Should_ReturnSuccess_When_CategoryExistAndNewNameIsUnique()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/categorys/image.jpg",
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>()).Returns(category);

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1).GetByIdAsync(Arg.Any<CategoryId>(), Arg.Any<CancellationToken>());

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
