using Overclocked.Domain.CategoryAggregate.Events;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.CategoryAggregate;

public sealed class Category : AggregateRoot<CategoryId>
{
    private Category()
    {
    }

    private Category(CategoryId id, string name, string imageUrl) : base(id)
    {
        Name = name;
        ImageUrl = imageUrl;

        NormalizedName = name.ToUpper();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public string ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Category Create(CategoryId id, string name, string imageUrl)
    {
        return new Category(
            id: id,
            name: name,
            imageUrl: imageUrl);
    }

    public void Update(string name, string imageUrl)
    {
        if(ImageUrl != imageUrl)
        {
            RaiseDomainEvent(new CategoryUpdatedEvent(Id, ImageUrl));
        }

        Name = name;
        ImageUrl = imageUrl;
        NormalizedName = name.ToUpper();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        RaiseDomainEvent(new CategoryDeletedEvent(Id, ImageUrl));
    }
}
