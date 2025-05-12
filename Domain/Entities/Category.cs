using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }

        // Navigations
        public ICollection<Product> Products { get; set; }
    }
}
