using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Address : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Discription { get; set; }

        // Navigations
        public Customer Customer { get; set; }
    }
}
