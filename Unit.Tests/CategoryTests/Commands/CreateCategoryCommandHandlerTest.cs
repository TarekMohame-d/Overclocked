using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Features.Category.Commands.CreateCategory;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CategoryTests.Commands;

public class CreateCategoryCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly CreateCategoryCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public CreateCategoryCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _mediatorMock = Substitute.For<IMediator>();
        _handler = new CreateCategoryCommandHandler(
            _unitOfWorkMock,
            _categoryRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Name = "Nike",
            ImageUrl = "image.png"
        };

        var category = new CategoryFaker().Generate();

        _categoryRepositoryMock.AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
            .Returns(category);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _categoryRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
