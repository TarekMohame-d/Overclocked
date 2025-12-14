using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.Results;
using Shouldly;

namespace Overclocked.Unit.Tests.CategoryTests;

public class CreateCategoryCommandHandlerTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateCategoryCommandHandler _createCategoryCommandHandler;

    public CreateCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

        _createCategoryCommandHandler = new CreateCategoryCommandHandler(_categoryRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateCategoryCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        Category category = new CategoryFaker().Generate();

        _categoryRepositoryMock.AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _createCategoryCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _categoryRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
