namespace Domain.Entities
{
    public class PaymentStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }

        // Navigations
        public ICollection<Payment> Payments { get; set; }
    }
}
