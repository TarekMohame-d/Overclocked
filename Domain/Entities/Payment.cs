using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        public string TransactionId { get; set; }
        public int PaymentMethodId { get; set; }
        public int StatusId { get; set; }
        public decimal Amount { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }

        // Navigations
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }
}
