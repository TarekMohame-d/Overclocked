namespace Domain.Entities
{
    public class PaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public string PaymentName { get; set; }

        // Navigations
        public ICollection<Payment> Payments { get; set; }
    }
}
