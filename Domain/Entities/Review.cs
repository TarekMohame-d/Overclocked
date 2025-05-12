using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public Product Product { get; set; }
    }
}
