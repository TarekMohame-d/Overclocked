using Domain.Entities.Common;

namespace Domain.Entities;

public class Specification : BaseEntity
{
    public Guid ProductId { get; set; }
    public required string Name { get; set; }
    public string NormalizedName { get; set; } = default!;
    public required string Value { get; set; }

    // Navigation Properties
    public Product? Product { get; set; }
}
