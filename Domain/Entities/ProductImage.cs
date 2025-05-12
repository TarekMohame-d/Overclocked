using Domain.Entities.Common;

namespace Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public Guid ProductId { get; set; }

        // Navigations
        public Product Product { get; set; }
    }
}
