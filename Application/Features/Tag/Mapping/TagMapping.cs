using Application.Features.Brand.Commands.UpdateBrand;
using Application.Features.Tag.Commands.CreateTag;
using Application.Features.Tag.Commands.UpdateTag;
using Application.Features.Tag.Queries.GetAllTags;
using Application.Features.Tag.Queries.GetTagById;
using TagEntity = Domain.Entities.Tag;

namespace Application.Features.Tag.Mapping;

public static class TagMapping
{
    public static TagEntity ToEntity(this CreateTagCommand command)
    {
        return new TagEntity
        {
            Name = command.Name
        };
    }

    public static void UpdateFrom(this TagEntity entity, UpdateTagWithIdCommand command)
    {
        entity.Name = command.Name;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static TagDto ToDto(this TagEntity entity)
    {
        return new TagDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public static IEnumerable<TagListDto> ToDto(this IEnumerable<TagEntity> entities)
    {
        return entities.Select(x => new TagListDto
        {
            Id = x.Id,
            Name = x.Name
        });
    }

    public static IQueryable<TagListDto> ToDto(this IQueryable<TagEntity> entities)
    {
        return entities.Select(x => new TagListDto
        {
            Id = x.Id,
            Name = x.Name
        });
    }
}
