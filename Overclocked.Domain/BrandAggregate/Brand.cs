using Overclocked.Domain.BrandAggregate.Events;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate;

public sealed class Brand : AggregateRoot<BrandId>
{
    private Brand()
    {
    }

    private Brand(BrandId id, string name, string imageUrl) : base(id)
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

    public static Brand Create(BrandId id, string name, string imageUrl)
    {
        return new Brand(
            id: id,
            name: name,
            imageUrl: imageUrl);
    }

    public void Update(string name, string imageUrl)
    {
        if(ImageUrl != imageUrl)
        {
            RaiseDomainEvent(new BrandUpdatedEvent(ImageUrl));
        }

        Name = name;
        ImageUrl = imageUrl;
        NormalizedName = name.ToUpper();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        RaiseDomainEvent(new BrandDeletedEvent(ImageUrl));
    }
}
