using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string BrandName { get; set; }
        public string BrandImage { get; set; }

        // Navigations
        public ICollection<Product> Products { get; set; }
    }
}
