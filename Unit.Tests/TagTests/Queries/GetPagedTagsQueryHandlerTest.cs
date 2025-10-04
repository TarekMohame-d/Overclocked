using System.Net;
using Application.Features.Tag.Mapping;
using Application.Features.Tag.Queries.GetPagedTags;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;
using MockQueryable;
using Application.Abstraction.Repositories;

namespace Unit.Tests.TagTests.Queries;

public class GetPagedTagsQueryHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly GetPagedTagsQueryHandler _handler;

    public GetPagedTagsQueryHandlerTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _handler = new GetPagedTagsQueryHandler(_tagRepositoryMock);
    }

    [Fact]
    public async Task Handle_WhenTagsExist_ShouldReturnTags()
    {
        // Arrange
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10
        };
        var tags = new TagFaker().Generate(3);

        var tagDtos = tags.ToDto().ToList();

        var mockQueryable = tags.BuildMock();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<string>())
            .Returns(mockQueryable);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        tags.Count.ShouldBe(result.Data.Items.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        tagDtos.ShouldBeEquivalentTo(result.Data.Items);

        _tagRepositoryMock.Received(1)
            .GetTagsQuery(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenTagsDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetPagedTagsQuery();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<string>())
            .Returns(new List<Tag>().BuildMock());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _tagRepositoryMock.Received(1)
            .GetTagsQuery(Arg.Any<string>());
    }
}
