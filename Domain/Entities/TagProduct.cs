namespace Domain.Entities;

public class TagProduct
{
    public Guid TagId { get; set; }
    public Guid ProductId { get; set; }

    // Navigation Properties
    public Tag? Tag { get; set; }
    public Product? Product { get; set; }
}
