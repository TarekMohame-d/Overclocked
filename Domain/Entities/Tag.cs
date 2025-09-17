using Domain.Entities.Common;

namespace Domain.Entities;

public class Tag : BaseEntity
{
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<TagProduct>? TagProducts { get; set; }
}
