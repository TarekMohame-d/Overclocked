using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public string Phone { get; set; }
        public Guid? WishListId { get; set; }
        public Guid CartId { get; set; }

        // Navigations
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public WishList WishList { get; set; }
        public Cart Cart { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Shipment> Shipments { get; set; }
    }
}
