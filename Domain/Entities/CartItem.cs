using Domain.Entities.Common;

namespace Domain.Entities;

public class CartItem : Entity
{
    public required Guid CartId { get; init; }
    public required Guid ProductId { get; init; }
    public required int Quantity { get; set; }

    // Navigation properties
    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
