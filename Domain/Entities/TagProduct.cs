namespace Domain.Entities;

public class TagProduct
{
    public required Guid TagId { get; set; }
    public required Guid ProductId { get; set; }

    // Navigation Properties
    public Tag? Tag { get; set; }
    public Product? Product { get; set; }
}
