using System.Text.Json.Serialization;
using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Enums;

namespace Application.Services.Tag.DTOs.Request;

public record GetPagedTagsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TagSortField SortBy { get; set; } = TagSortField.Id;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection Direction { get; set; } = SortDirection.Asc;
}

public record GetPagedTagsQuery : GetPagedTagsRequest, ICachedRequest
{
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SortBy.ToString(), Direction.ToString());
    public string? CacheSetKey => CacheKeys.TagSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);

    public static GetPagedTagsQuery FromRequest(GetPagedTagsRequest request)
    {
        return new GetPagedTagsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            Direction = request.Direction
        };
    }
}

