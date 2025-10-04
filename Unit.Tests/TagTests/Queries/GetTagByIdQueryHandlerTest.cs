using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using Application.Features.Tag.Queries.GetTagById;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.TagTests.Queries;

public class GetTagByIdQueryHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly GetTagByIdQueryHandler _handler;

    public GetTagByIdQueryHandlerTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _handler = new GetTagByIdQueryHandler(_tagRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldReturnTag()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var query = new GetTagByIdQuery { Id = tagId };
        var tag = new TagFaker().Generate();
        var tagDto = tag.ToDto();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        tagDto.ShouldBeEquivalentTo(result.Data);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var query = new GetTagByIdQuery { Id = tagId };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
