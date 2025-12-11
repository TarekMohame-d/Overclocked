using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Tag.Queries;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate;
using Shouldly;
using SortDirection = Overclocked.Application.Common.Enums.SortDirection;

namespace Overclocked.Unit.Tests.TagTests;

public class GetPagedTagsQueryHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly ITagQueries _tagQueries;

    public GetPagedTagsQueryHandlerTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagQueries = new TagQueries(_tagRepositoryMock);
    }

    [Fact]
    public async Task GetPagedTagsQueryHandler_Should_ReturnEmptyList_When_NoTagsFoundBySearchTerm()
    {
        // Arrange
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc"
        };

        _tagRepositoryMock.CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(0);

        // Act
        Result<PagedResult<TagPagedResponse>> result = await _tagQueries
            .GetPagedTagsQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldBeEmpty();
        result.Value.HasNextPage.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _tagRepositoryMock.Received(1)
            .CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnTags_When_TagsFoundWithSearchTerm()
    {
        // Arrange
        List<Tag> tags = new TagFaker().Generate(3);
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = tags[0].Name,
            SortBy = "id",
            Direction = "asc"
        };

        _tagRepositoryMock.CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(10);

        _tagRepositoryMock.GetTagsAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<TagSortField>(),
            Arg.Any<SortDirection>())
            .Returns(tags);

        // Act
        Result<PagedResult<TagPagedResponse>> result = await _tagQueries
            .GetPagedTagsQueryHandler(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.Items.ShouldNotBeEmpty();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _tagRepositoryMock.Received(1)
            .CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1)
            .GetTagsAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<TagSortField>(),
            Arg.Any<SortDirection>());
    }
}
