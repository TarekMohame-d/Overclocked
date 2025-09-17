using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public required string Image { get; set; }

        // Navigation Properties
        public ICollection<Product>? Products { get; set; }
    }
}
