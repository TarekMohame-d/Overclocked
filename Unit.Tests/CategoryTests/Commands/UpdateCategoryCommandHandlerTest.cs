using System.Net;
using Application.Common.Results;
using Application.Features.Category.Commands.UpdateCategory;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Repositories;
using Application.Abstraction.Messaging;
using Domain.Entities;
using Application.Abstraction.Services;

namespace Unit.Tests.CategoryTests.Commands;

public class UpdateCategoryCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly UpdateCategoryCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public UpdateCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _mediatorMock = Substitute.For<IMediator>();

        _handler = new UpdateCategoryCommandHandler(
            _unitOfWorkMock,
            _categoryRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Category)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _categoryRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryExist_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var category = new CategoryFaker().Generate();

        category.Name = command.Name;

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _categoryRepositoryMock.Update(Arg.Any<Category>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

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

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
