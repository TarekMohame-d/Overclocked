using System.Net;
using System.Net.Http.Json;
using Api.Common.Routing;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using Application.Services;
using Application.Services.Tag.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;


namespace Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class GetPagedTagsTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetPagedTagsTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        var tags = await SeedDatabaseAsync();

        // Act
        var url = $"{TagRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count().ShouldBe(10);
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        var tagListDtos = await SeedCacheAsync();

        // Act
        var url = $"{TagRoutes.GetAll}?Page=1&PageSize=20";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count().ShouldBe(tagListDtos.Count());
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange
        IEnumerable<Tag> tags = [];

        // Act
        var url = $"{TagRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(tags.Count());
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        var tags = await SeedDatabaseAsync();

        int concurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var url = $"{TagRoutes.GetAll}?Page=1&PageSize=5&SortBy=name&Direction=asc";
            tasks.Add(_client.GetAsync(url));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.Items.ShouldNotBeEmpty();
            result.Data.Items.Count().ShouldBe(5);
        }
    }

    private async Task<IEnumerable<Tag>> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var tags = new TagFaker().Generate(20);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return tags;
    }

    private async Task<IEnumerable<TagListResponse>> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var tags = new TagFaker().Generate(20);

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.TagPaged(1, 20, "Id", "Asc");
        var tagListDtos = tags.ToDto();
        PagedResult<TagListResponse> pagedResult = new PagedResult<TagListResponse>
        {
            Items = tagListDtos.ToList(),
            PageNumber = 1,
            PageSize = 20,
            TotalItemCount = tags.Count()
        };
        var result = Result<PagedResult<TagListResponse>>.Success(pagedResult);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return tagListDtos;
    }
}
