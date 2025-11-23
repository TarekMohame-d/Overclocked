using Domain.Entities.Common;

namespace Domain.Entities;

public class ProductImage : Entity
{
    public required Guid ProductId { get; set; }
    public required string Image { get; set; }

    // Navigation Properties
    public Product? Product { get; set; }
}
