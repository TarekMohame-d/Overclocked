using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Shipment : BaseEntity
    {
        public DateTime ShipmentDate { get; set; }
        public Guid AddressId { get; set; }
        public Guid CustomerId { get; set; }

        // Navigations
        public Address Address { get; set; }
        public Customer Customer { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
