using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class GetPagedTagsTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Tag> tags = await SeedDatabaseAsync();

        // Act
        const string Url = $"{TagRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<TagListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count.ShouldBe(10);
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        IEnumerable<TagListResponse> tagListDtos = await SeedCacheAsync();

        // Act
        const string Url = $"{TagRoutes.GetAll}?Page=1&PageSize=20";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<TagListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldNotBeEmpty();
        result.Data.Items.Count.ShouldBe(tagListDtos.Count());
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        const string Url = $"{TagRoutes.GetAll}?Page=1&PageSize=10&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<PagedResult<TagListResponse>>? result =
            await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(0);
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Tag> tags = await SeedDatabaseAsync();

        const int ConcurrentCalls = 10;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < ConcurrentCalls; i++)
        {
            const string Url = $"{TagRoutes.GetAll}?Page=1&PageSize=5&SortBy=name&Direction=asc";
            tasks.Add(_client.GetAsync(Url));
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<PagedResult<TagListResponse>>? result =
                await response.Content.ReadFromJsonAsync<Result<PagedResult<TagListResponse>>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
            result.Data.Items.ShouldNotBeEmpty();
            result.Data.Items.Count.ShouldBe(5);
        }
    }

    private async Task<IEnumerable<Tag>> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Tag> tags = new TagFaker().Generate(20);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return tags;
    }

    private async Task<IEnumerable<TagListResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Tag> tags = new TagFaker().Generate(20);

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.TagPaged(1, 20, "Id", "Asc");
        IEnumerable<TagListResponse> tagListDtos = tags.ToDto();
        var pagedResult = new PagedResult<TagListResponse>
        {
            Items = tagListDtos.ToList(),
            PageNumber = 1,
            PageSize = 20,
            TotalItemCount = tags.Count
        };
        var result = Result<PagedResult<TagListResponse>>.Success(pagedResult);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return tagListDtos;
    }
}
