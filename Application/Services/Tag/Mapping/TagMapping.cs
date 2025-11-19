using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using TagEntity = Domain.Entities.Tag;

namespace Application.Services.Tag.Mapping;

public static class TagMapping
{
    public static TagEntity ToEntity(this CreateTagRequest request) => new TagEntity { Name = request.Name };

    public static void UpdateFrom(this TagEntity entity, UpdateTagRequest request)
    {
        entity.Name = request.Name;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static TagResponse ToDto(this TagEntity entity) => new() { Id = entity.Id, Name = entity.Name };

    public static IEnumerable<TagListResponse> ToDto(this IEnumerable<TagEntity> entities) =>
        entities.Select(x => new TagListResponse { Id = x.Id, Name = x.Name });

    public static IQueryable<TagListResponse> ToDto(this IQueryable<TagEntity> entities) =>
        entities.Select(x => new TagListResponse { Id = x.Id, Name = x.Name });
}
