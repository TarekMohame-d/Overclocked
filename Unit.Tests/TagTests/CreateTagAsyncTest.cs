using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.TagTests;

public class CreateTagAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly TagService _tagService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateTagAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagService = new TagService(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateTagAsync_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new CreateTagRequest { Name = "Tag Name" };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>()).Returns(tag);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _tagService.CreateTagAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _tagRepositoryMock.Received(1).AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
