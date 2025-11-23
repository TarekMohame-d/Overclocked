using Domain.Entities.Common;

namespace Domain.Entities;

public class Brand : Entity
{
    public required string Name { get; set; }
    public string NormalizedName { get; } = string.Empty;
    public required string Image { get; set; }

    // Navigation Properties
    public ICollection<Product> Products { get; set; } = [];
}
