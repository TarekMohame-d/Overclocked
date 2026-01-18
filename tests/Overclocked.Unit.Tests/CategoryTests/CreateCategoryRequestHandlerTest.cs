using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CategoryUseCases.CreateCategory;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class CreateCategoryRequestHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateCategoryRequestHandler _createCategoryRequestHandler;

    public CreateCategoryRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _createCategoryRequestHandler = new CreateCategoryRequestHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateCategoryRequestHandler_Should_ReturnFailure_When_NameAlreadyExists()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _createCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCategoryRequestHandler_Should_ReturnFailure_When_ImageUrlIsInvalid()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.txt",
        };

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCategoryRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "  ", ImageUrl = "https://res.cloudinary.com/over-clocked/image.txt" };

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCategoryRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _categoryRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createCategoryRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1).Add(Arg.Any<Category>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
