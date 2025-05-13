using Domain.Entities.Common;

namespace Domain.Entities
{
    public class CartProduct : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Guid CartId { get; set; }

        // Navigations
        public Product Product { get; set; }
        public Cart Cart { get; set; }
    }
}
