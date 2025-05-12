namespace Domain.Entities
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public int StatusId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderId { get; set; }

        // Navigations
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }
}
