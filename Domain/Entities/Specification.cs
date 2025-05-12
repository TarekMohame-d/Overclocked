using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Specification : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        // Navigations
        public Product Product { get; set; }
    }
}
