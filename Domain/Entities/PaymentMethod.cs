namespace Domain.Entities
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }

        // Navigations
        public ICollection<Payment> Payments { get; set; }
    }
}
