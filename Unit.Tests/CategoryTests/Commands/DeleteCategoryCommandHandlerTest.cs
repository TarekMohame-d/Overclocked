using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Category.Commands.DeleteCategory;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Repositories;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests.Commands;

public class DeleteCategoryCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly DeleteCategoryCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public DeleteCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mediatorMock = Substitute.For<IMediator>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _handler = new DeleteCategoryCommandHandler(
            _unitOfWorkMock,
            _categoryRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteCategoryCommand();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = new DeleteCategoryCommand();

        var category = new CategoryFaker().Generate();

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.Delete(Arg.Any<Category>());

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        _categoryRepositoryMock.Received(1)
            .Delete(Arg.Any<Category>());
    }
}
