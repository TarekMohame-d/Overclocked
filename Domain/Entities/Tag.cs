using Domain.Entities.Common;

namespace Domain.Entities;

public class Tag : Entity
{
    public required string Name { get; set; }
    public string NormalizedName { get; set; } = default!;

    // Navigation Properties
    public ICollection<TagProduct> TagProducts { get; set; } = [];
}
