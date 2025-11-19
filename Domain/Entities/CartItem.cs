using Domain.Entities.Common;

namespace Domain.Entities;

public class CartItem : Entity
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public Cart? Cart { get; set; }
    public Product? Product { get; set; }
}
